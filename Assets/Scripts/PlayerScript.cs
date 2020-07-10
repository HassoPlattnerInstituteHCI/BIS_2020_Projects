using System;
using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using SpeechIO;
using DualPantoFramework;

namespace PantoGolf
{
    public class PlayerScript : MonoBehaviour
    {

        public bool allowMovement = true;
        public bool disableHit = false;
        private GameObject Ball;
        private Collider m_Collider;
        public float forceMultiplier = 1f;  //Multiplier to increase hitstrength
        public float upForce = 5f;
        private float velocity = 0f;     //Stores the velocity of the moving club
        public float minHitStrength = 10f;
        private BallAudio soundEffects;

        private int hitCount = 0;

        public SpeechOut speechOut = new SpeechOut();
        private Vector3 previousPosition;   //To calculate velocity of club.

        private const int pauseAfterHit = 100;
        private int pauseCounter = 0;

        // Start is called before the first frame update
        void Start()
        {
            Ball = GameObject.FindGameObjectWithTag("ball");
            m_Collider = GetComponent<Collider>();
            soundEffects = Ball.GetComponent<BallAudio>();
            previousPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            //transform.position = GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position);

            //GameObject.Find("Panto").GetComponent<UpperHandle>().
            if (pauseCounter <= 0)
            {
                ReadyToHit();
            }
            else
            {
                pauseCounter--;
            }
        }

        private void FixedUpdate()
        {
            //Move Club to Handle Position
            if (allowMovement)
            {
                StartCoroutine(MoveOverSpeed(GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position), 100));
                //Rotate Club according to Handle Position
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x,
                    GameObject.Find("Panto").GetComponent<UpperHandle>().GetRotation(),
                    transform.eulerAngles.z
                    );
                velocity = (transform.position - previousPosition).magnitude / Time.deltaTime;
            }
            previousPosition = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == Ball && m_Collider.enabled)
            {
                Debug.Log("Hitting the Ball, Collider disabled.");
                hitCount++;
                GameManager.totalHitCount++;
                pauseCounter = pauseAfterHit;   //Constant time to wait after a hit to check if ball is moving
                                                //Calculate Angle vertical to the club:
                Vector3 shotDir = new Vector3(0, 0, 0);
                float angle = transform.eulerAngles.y * Mathf.Deg2Rad;
                shotDir.x = Mathf.Cos(angle);
                shotDir.z = -Mathf.Sin(angle);
                //shotDir is vertical to the club but is always facing to the right -> if the ball is on the left side of the club, it will go through the club (to the right)
                Vector3 ballDir = Ball.transform.position - transform.position;
                ballDir.y = 0;
                //ballDir is the vector from the club to the goal.
                float ballAngle = Vector3.Angle(ballDir, shotDir);
                if (ballAngle > 90) //Check if the ball is on the left side of the club.
                {
                    //Debug.Log("Ball is on the left side of the Club. Inverting shotDir.");
                    shotDir = -shotDir;
                }
                if (velocity < minHitStrength)
                {
                    Debug.Log("Hitting Ball with minHitStrength of: " + minHitStrength.ToString() + " in direction: " + shotDir);
                    Ball.GetComponent<Rigidbody>().AddForce(shotDir.normalized * forceMultiplier * minHitStrength);
                }
                else
                {
                    Debug.Log("Hitting Ball with velocity of: " + velocity.ToString() + " in direction: " + shotDir);
                    Ball.GetComponent<Rigidbody>().AddForce(shotDir.normalized * forceMultiplier * velocity);
                }
                soundEffects.PlayClubHit();
                soundEffects.PlayRolling(1f);
                m_Collider.enabled = false;
                Debug.Log("End of Player Trigger Event");
            }
        }

        public IEnumerator MoveOverSpeed(Vector3 end, float speed)
        {
            // speed should be 1 unit per second
            while (transform.position != end)
            {
                transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

        protected virtual bool ReadyToHit()
        {
            if (!m_Collider.enabled)
            {
                if (disableHit)
                {
                    return false;
                }
                Rigidbody rb = Ball.GetComponent<Rigidbody>();
                if (rb.velocity.magnitude > 0.02)   //Check if ball is moving
                {
                    return false;
                }
                if (Ball.GetComponent<BallScript>().enabled == false)
                {
                    return false;
                }
                Debug.Log("Ball is not moving:");
                // Ball is not moving anymore:

                rb.velocity = Vector3.zero;     //Balls velocity set to 0.

                soundEffects.StopRolling();
                int nexthit = hitCount + 1;
                VoiceOut("Waiting for hit " + nexthit);
                soundEffects.PlayReadyToHit();
                m_Collider.enabled = true;      //Enable Club to make next hit.
                Debug.Log("Collider enabled.");
            }
            return true;
        }

        private async void VoiceOut(string message)
        {
            speechOut.Stop();
            await speechOut.Speak(message);
        }
    }
}