using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;

namespace MarioKart
{
    public class DraggedPlayer : MonoBehaviour
    {
        private DualPantoSync pantoSync;
        private PauseManager pauseManager;
        private PantoHandle handle;
        [Header("Movement")]
        public GameObject startPos;
        public float deadZone = 0.1f;
        public float maxSpeed = 5.0f;
        public float maxHandleDistance = 5.0f;
        public float curveLeadLength = 1.0f;
        private Rigidbody rigidbody;
        private AudioSource motorSound;
        public AnimationCurve motorSoundVolumeNorm;
        public float maxMotorSoundVolume = 0.5f;
        public AnimationCurve motorSoundPitchNorm;
        public float maxMotorSoundPitch = 2.0f;
        [Header("Contraints")]
        public PathCreation.Examples.RoadMeshCreator roadMeshCreator;
        public GameObject meHandleTarget;
        public AnimationCurve vibrationStrengthCurve;
        public float maxVibrationStrength = 0.5f;
        public AudioSource scratchSound;

        [Header("Debug")]
        public GameObject marker;
        private float speedNorm = 0.0f;


        async void Start()
        {
            pantoSync = GameObject.Find("Panto").GetComponent<DualPantoSync>();
            handle = pantoSync.GetComponent<UpperHandle>();
            rigidbody = GetComponent<Rigidbody>();
            motorSound = GetComponent<AudioSource>();
            pauseManager = GetComponent<PauseManager>();
            pauseManager.OnPauseChanged += OnPauseChangedHandler;
            if (!pantoSync.debug)
            {
                await Task.Delay(2000);
                if (!pauseManager.isPaused)
                {
                    CaptureHandle();
                }
            }
        }

        public async void Reset()
        {
            transform.position = startPos.transform.position;
        }

        async void CaptureHandle()
        {
            if (pantoSync.debug)
            {
                return;
            }
            await handle.SwitchTo(meHandleTarget, 0.2f);
        }

        void OnPauseChangedHandler(bool isPaused)
        {
            if (isPaused)
            {
                rigidbody.Sleep();
            }
            else
            {
                rigidbody.WakeUp();
                CaptureHandle();
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (pauseManager.isPaused)
            {
                rigidbody.Sleep();
                return;
            }

            MoveToHandle();
            PlayMotorSound();
            GiveDistanceFeedback();
        }

        void MoveToHandle()
        {
            Vector3 movement = GetVector();
            if (movement.magnitude < deadZone)
            {
                rigidbody.velocity = Vector3.zero;
                return;
            }
            speedNorm = Mathf.Clamp(movement.magnitude / maxHandleDistance, 0.0f, 1.0f);
            float actualSpeed = speedNorm * (IsOnGrass() ? maxSpeed / 2 : maxSpeed);
            rigidbody.velocity = movement.normalized * actualSpeed * Time.fixedDeltaTime;
            LeadCurve();
        }

        void LeadCurve()
        {
            Vector3 curveLeadPos = GetCurveLeadPosition();
            Vector3 delta = curveLeadPos - handle.GetPosition();
            Quaternion rotation = Quaternion.LookRotation(delta);
            transform.rotation = rotation;
        }

        Vector3 GetVector()
        {
            Vector3 handlePos = handle.GetPosition();
            return handlePos - transform.position;
        }

        void PlayMotorSound()
        {
            float volume = motorSoundVolumeNorm.Evaluate(speedNorm) * maxMotorSoundVolume;
            float pitch = motorSoundPitchNorm.Evaluate(speedNorm) * maxMotorSoundPitch;
            motorSound.volume = volume;
            motorSound.pitch = pitch;
        }

        // Gets the distance of the me handle to the center of the road and vibrates the position accordingly
        void GiveDistanceFeedback()
        {
            float distance = GetRoadCenterDistance();
            float normalized = distance / roadMeshCreator.roadWidth;
            float vibrationStrength = Mathf.Clamp(vibrationStrengthCurve.Evaluate(normalized), 0, 1) * maxVibrationStrength;
            Vibrate(vibrationStrength);
        }
        void Vibrate(float strength)
        {
            Vector3 random = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;
            meHandleTarget.transform.position = transform.position + random * strength;
            scratchSound.volume = strength;
        }

        float GetRoadCenterDistance()
        {
            Vector3 meHandlePosition = handle.GetPosition();
            Vector3 closest = roadMeshCreator.pathCreator.path.GetClosestPointOnPath(meHandlePosition);
            float distance = Vector3.Distance(meHandlePosition, closest);
            return distance;
        }

        Vector3 GetCurveLeadPosition()
        {
            Vector3 handlePos = handle.GetPosition();
            float pointOnCurve = roadMeshCreator.pathCreator.path.GetClosestDistanceAlongPath(handlePos);
            pointOnCurve += curveLeadLength;
            Vector3 leadPos = roadMeshCreator.pathCreator.path.GetPointAtDistance(pointOnCurve);
            marker.transform.position = leadPos;
            return leadPos;
        }

        bool IsOnGrass()
        {
            Vector3 closestPointOnPath = roadMeshCreator.pathCreator.path.GetClosestPointOnPath(transform.position);
            float distance = (transform.position - closestPointOnPath).magnitude;
            return distance >= roadMeshCreator.roadWidth;
        }
    }
}