﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace PantoGolf
{
    public class BallScript : MonoBehaviour
    { 
        private Rigidbody rb;
        private GameObject Ball;
        private BallAudio soundEffects;
        private LowerHandle LowerHandle;
        public bool active = true;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            Ball = GameObject.Find("Ball");
            soundEffects = GetComponent<BallAudio>();
            LowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            //StartCoroutine(LowerHandle.SwitchTo(Ball, 0.2f));
        }

        // Update is called once per frame
        void Update()
        {
            //Make the ball look at the goal, so the ItHandle faces the direction of the goal as well!
            Ball.transform.LookAt(GameObject.Find("Goal").transform.position);
            if (transform.position.y < -1 && active)  //Somehow the ball might shoot through the wall.
            //if this happens, we want to restart the level.
            {
                active = false;
                Debug.LogError("Ball is outside the play area!");
                soundEffects.voiceOutput("An error occured.");
                StartCoroutine(RestartLevel());
            }
        }

        private void FixedUpdate()
        {
            //StartCoroutine(GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(Ball, 0.2f));
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Ball Collision.");
            if (collision.gameObject.CompareTag("obstacle"))
            {
                Debug.Log("Ball hit wall/obstacle.");
                soundEffects.PlayObstacle();
            }
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("goal"))
            {
                Debug.Log("The ball hit the goal!");
                active = false;
                GameObject.Find("Player").GetComponent<PlayerScript>().disableHit = true; //This disables the "readyToHit sound for the player
                soundEffects.PlayGoal();
                rb.velocity = Vector3.zero;
                // Start next level
                StartCoroutine(levelComplete());
                // Start next Level
            }
            else if (other.gameObject.CompareTag("water"))
            {
                Debug.Log("Ball fell in water!");
                active = false;
                GameObject.Find("Player").GetComponent<PlayerScript>().disableHit = true;  //This disables the "readyToHit sound for the player
                rb.velocity = Vector3.zero;
                soundEffects.PlayWaterDrop();
                StartCoroutine(RestartLevel());
            }
        }


        IEnumerator RestartLevel()
        {
            yield return new WaitForSeconds(2);
            GameObject.Find("Panto").GetComponent<GameManager>().RestartLevel();
        }

        IEnumerator levelComplete()
        {
            yield return new WaitForSeconds(2);
            GameObject.Find("Panto").GetComponent<GameManager>().LevelComplete();
        }
    }
}
