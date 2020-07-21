using System;
using SpeechIO;
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
        public float health = 100.0f;

        public float HitPower = 10.0f;

        // The enemy will patrol between these points;
        public Vector3[] path;
        public float SpotRadius = 4.0f;

        public GameObject player;

        // The part of the path the enemy is currently headed to
        private int currentPathTargetIndex = 0;
        private bool spotted = false;
        SpeechOut speechOut;
        public AudioSource failureAudioSource;
        public AudioSource death;
        public AudioSource dangerAudioSource;
        public bool canSpot = false;
        public bool frozen = true;

        void Start()
        {
            speechOut = new SpeechOut();
        }

        // Update is called once per frame
        void Update()
        {
            if (isFrozen())
            {
                return;
            }

            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= SpotRadius && !spotted)
            {
                spotted = true;
                PlayerSpotted();
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
            LevelManager script = GameObject.Find("Panto").GetComponent<LevelManager>();

            script.FreezeGameObjects();
            if (SceneManager.GetActiveScene().name != "Level 5")
            {
                await speechOut.Speak(gameObject.name + " has spotted you. Try again.");
            }


            if (SceneManager.GetActiveScene().name != "Level 5")
            {
                await script.ResetGame();
                spotted = false;
            }
            else
            {
                Debug.Log("ACTIVATE SWORD");
                player.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        public void TakeHit()
        {
            if (health > 0)
            {
                health = health - HitPower;
                Debug.Log("helath is " + health);
            }

            if (health == 0)
            {
                EnemyDeath();
            }
        }

        async Task EnemyDeath()
        {
            Debug.Log("death");
            GameObject.Find("Sword").SetActive(false);
            death.Play();
            await speechOut.Speak(gameObject.name + " has died");
            gameObject.SetActive(false);
        }

        public Boolean isFrozen()
        {
            return frozen;
        }

        public void Freeze()
        {
            frozen = true;
            dangerAudioSource.enabled = false;
        }

        public void Unfreeze()
        {
            frozen = false;
            gameObject.SetActive(true);
            dangerAudioSource.enabled = true;
        }
    }
}