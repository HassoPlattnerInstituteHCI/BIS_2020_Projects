using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using SpeechIO;
using MarioKart;
using DualPantoFramework;

namespace MarioKart
{
    public class Player : MonoBehaviour
    {
        public PathCreator pathCreator;
        public GameObject tracker;
        public GameObject trackerPos;
        private GameObject panto;
        private PantoHandle meHandle;
        private PantoHandle itHandle;
        public GameObject desired;
        private Rigidbody rigidBody;
        public float defaultSpeed = 10.0f;
        public float speed;
        public float maxMeDistance = 1.0f;
        [Range(0.0f, 0.5f)]
        public float handleSpeed = 0.5f;
        private PauseManager pauseManager;

        // Start is called before the first frame update
        void Start()
        {
            speed = defaultSpeed;
            panto = GameObject.Find("Panto");
            meHandle = panto.GetComponent<UpperHandle>();
            itHandle = panto.GetComponent<LowerHandle>();
            rigidBody = GetComponent<Rigidbody>();
            pauseManager = GetComponent<PauseManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (pauseManager.isPaused)
            {
                rigidBody.Sleep();
                return;
            }

            rigidBody.WakeUp();

            MoveTracker();
            RotateToMe();
            ApplyForce();
        }

        void MoveTracker()
        {
            Vector3 closest = pathCreator.path.GetClosestPointOnPath(trackerPos.transform.position);
            tracker.transform.position = closest;
        }

        void RotateToMe()
        {
            float myAngle = transform.rotation.eulerAngles.y;
            float angle = meHandle.GetRotation();
            float angleDiff = angle - myAngle;
            Vector3 newVel = Quaternion.Euler(0, angleDiff, 0) * rigidBody.velocity;
            transform.eulerAngles = new Vector3(0, angle, 0);
            rigidBody.velocity = newVel;
        }

        float GetAccelleration()
        {
            Vector3 normal = transform.rotation * Vector3.forward * maxMeDistance;
            Vector3 desired = meHandle.GetPosition() - transform.position;
            float factor = Vector3.Dot(normal, desired);
            // MoveMeTo(transform.position + Vector3.ClampMagnitude(desired, maxMeDistance));
            return factor;
        }

        async void MoveMeTo(Vector3 position)
        {
            desired.transform.position = position;
            await meHandle.SwitchTo(desired, handleSpeed);
            meHandle.Free();
        }

        void ApplyForce()
        {
            float speedFactor = GetAccelleration();
            speedFactor = Mathf.Clamp(speedFactor, -speed, speed);
            Vector3 velocity = transform.rotation * Vector3.forward * speedFactor * speed;
            rigidBody.velocity = velocity;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Checkpoint"))
            {
                GameObject.FindObjectOfType<Goal>().NextCheckpoint();
                Destroy(other.gameObject);
            }

            else if (other.gameObject.CompareTag("Goal"))
            {
                GameObject.FindObjectOfType<Goal>().NextLap();
            }
        }
    }

}