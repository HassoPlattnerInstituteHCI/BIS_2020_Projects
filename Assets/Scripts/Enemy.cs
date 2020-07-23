using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

namespace MarioKart
{
    public class Enemy : MonoBehaviour
    {
        private Rigidbody enemyRb;
        public PathCreator pathCreator;

        private int laps = 0;
        Boolean crash = false;
        public float crashTime = 2.0f;

        public MovementGoal movementTarget;
        public float maxMovementSpeed = 1.0f;
        PauseManager pauseManager;
        public float bounceStength = 0.1f;
        private AudioSource audioSource;
        public AudioClip hitClip;

        // Start is called before the first frame update
        private void Start()
        {
            enemyRb = GetComponent<Rigidbody>();
            pauseManager = GetComponent<PauseManager>();
            audioSource = GetComponent<AudioSource>();
            Reset();
        }

        public void Reset()
        {
            movementTarget.Reset();
            transform.position = movementTarget.transform.position;
        }

        // void Update()
        // {
        //     distance += speed * Time.deltaTime;
        //     transform.position = pathCreator.path.GetPointAtDistance(distance, end);
        //     transform.rotation = pathCreator.path.GetRotationAtDistance(distance, end);

        //     /*distance += speed * Time.deltaTime;
        //     wantedPoint = pathCreator.path.GetPointAtDistance(distance, end);
        //     Vector3 direction = wantedPoint - transform.position;
        //     direction.y = 0;
        //     enemyRb.AddForce((1/(Mathf.Sqrt(direction.x*direction.x + direction.z*direction.z))*speed)*direction);
        //     */
        // }

        void FixedUpdate()
        {
            if (pauseManager.isPaused)
            {
                enemyRb.Sleep();
                return;
            }
            MoveToTarget();
        }

        public void ModifySpeed(float modifier)
        {
            maxMovementSpeed *= modifier;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //sound
                if (!crash)
                {

                    StartCoroutine(SlowDown(crashTime));
                }
                print("Hit player!");
                // Push back player and enemy
                audioSource.PlayOneShot(hitClip);
                Vector3 toPlayer = (other.transform.position - transform.position).normalized;
                enemyRb.AddForce(-toPlayer * bounceStength, ForceMode.VelocityChange);
                other.GetComponent<Rigidbody>().AddForce(toPlayer * bounceStength, ForceMode.VelocityChange);

            }
        }

        IEnumerator SlowDown(float timeout)
        {
            crash = true;
            ModifySpeed(0.5f);
            yield return new WaitForSeconds(timeout);
            ModifySpeed(2.0f);
            crash = false;
        }

        void MoveToTarget()
        {
            Vector3 direction = (movementTarget.transform.position - transform.position).normalized;
            enemyRb.velocity = direction * maxMovementSpeed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
