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
        public EndOfPathInstruction end;
        float distance;
        public float speed;
        Vector3 wantedPoint;
        private int laps = 0;
        Boolean crash = false;
        int crashFrames = 20;
        int frameCounter = 0;
        // Start is called before the first frame update
        private void Start()
        {
            enemyRb = GetComponent<Rigidbody>();
            distance = -5;
        }

        public void Spawn()
        {
            laps = 0;
            //frameCounter = 0;
            //crash = false;
            //speed = 2;
            distance = -5;
            transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length + distance, end);
            transform.rotation = pathCreator.path.GetRotationAtDistance(pathCreator.path.length + distance, end);

        }

        void Update()
        {
            distance += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distance, end);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distance, end);

            /*distance += speed * Time.deltaTime;
            wantedPoint = pathCreator.path.GetPointAtDistance(distance, end);
            Vector3 direction = wantedPoint - transform.position;
            direction.y = 0;
            enemyRb.AddForce((1/(Mathf.Sqrt(direction.x*direction.x + direction.z*direction.z))*speed)*direction);
            */
        }

        void FixedUpdate()
        {
            if (crash)
                if (frameCounter++ == crashFrames)
                {
                    ModifySpeed(2);
                    crash = false;
                    frameCounter = 0;
                }
        }

        public void ModifySpeed(float modifier)
        {
            speed = modifier * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            print("Collision");
            if (other.gameObject.CompareTag("Goal"))
            {
                laps++;
                if (laps == other.gameObject.GetComponent<Goal>().getMaxLaps())
                {
                    other.gameObject.GetComponent<Goal>().increasePlace();
                    Destroy(gameObject);
                }
            }

            if (other.gameObject.CompareTag("Player"))
            {
                //sound
                ModifySpeed(0.5f);
                crash = true;
                print("Hit player!");
            }
        }
    }
}
