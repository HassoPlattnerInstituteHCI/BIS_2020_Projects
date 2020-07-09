﻿using SpeechIO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class EnemyController : MonoBehaviour
    {
        // Start is called before the first frame update
        public float speed = 1.0f;
        // The enemy will patrol between these points;
        public Vector3[] path;
        public float SpotRadius = 4.0f;
        public GameObject player;
        // The part of the path the enemy is currently headed to
        private int currentPathTargetIndex = 0;
        private bool spotted = false;
        SpeechOut speechOut;
        public AudioSource failureAudioSource;
        public bool canSpot = false;

        void Start()
        {
            speechOut = new SpeechOut();
            


        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= SpotRadius && !spotted)
            {
                if (canSpot)
                {
                  spotted = true;
                  PlayerSpotted();
                }
            }
            if (gameObject.transform.position == getCurrentTarget())
            {
                currentPathTargetIndex = (currentPathTargetIndex + 1) % path.Length;
            }

            moveTowardsTarget();
        }

        void moveTowardsTarget()
        {
            gameObject.transform.position =
                Vector3.MoveTowards(gameObject.transform.position, getCurrentTarget(), Time.deltaTime * speed);
        }

        Vector3 getCurrentTarget()
        {
            return path[currentPathTargetIndex];
        }
        async Task PlayerSpotted()
        {
            failureAudioSource.Play();
            await speechOut.Speak(gameObject.name + " has spotted you. Try again.");
            Debug.Log("Making a call");
            
            LevelManager script = GameObject.Find("Panto").GetComponent<LevelManager>();

            await script.ResetGame();
            spotted = false;
        }
    }
}