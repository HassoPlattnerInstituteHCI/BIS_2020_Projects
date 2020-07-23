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
        public int rotateSpeed = 60;

        // The enemy will patrol between these points
        // NB: They are relative to the starting position.;
        public Vector3[] path;
        public float SpotRadius = 4.0f;

        public GameObject player;

        // The part of the path the enemy is currently headed to
        private int currentPathTargetIndex = 0;
        private Vector3 startingPosition;
        private bool spotted = false;
        SpeechOut speechOut;
        public AudioSource failureAudioSource;
        public AudioSource death;
        public AudioSource dangerAudioSource;
        public bool frozen = true;
        public GameObject sword;
        private bool fight = false;

        void Start()
        {
            speechOut = new SpeechOut();
            startingPosition = gameObject.transform.position;
        }

        LevelManager GetLevelManager()
        {
            return GameObject.Find("Panto").GetComponent<LevelManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsFrozen())
            {
                return;
            }

            if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= SpotRadius && !spotted && !fight)
            {
                spotted = true;
                PlayerSpotted();
            }

            if (gameObject.transform.position == GetCurrentTarget() && !fight)
            {
                currentPathTargetIndex = (currentPathTargetIndex + 1) % path.Length;
            }

            MoveTowardsTarget();
            if (fight)
            {
                RotateWithSword();
            }
        }

        void MoveTowardsTarget()
        {
            gameObject.transform.position =
                Vector3.MoveTowards(gameObject.transform.position, GetCurrentTarget(), Time.deltaTime * speed);
        }
        void RotateWithSword()
        {
            Vector3 rot = new Vector3(0, rotateSpeed * Time.deltaTime, 0);
            gameObject.transform.Rotate(rot);
        }

        Vector3 GetCurrentTarget()
        {
            if (fight)
            {
                if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= 1.0f)
                {
                    return transform.position;
                }
                return player.transform.position;
            }

            else
                return path[currentPathTargetIndex] + startingPosition;
        }

        async Task PlayerSpotted()
        {
            failureAudioSource.Play();
            LevelManager script = GetLevelManager();

            
            if (SceneManager.GetActiveScene().name != "Level 5")
            {
                script.FreezeGameObjects();
                await GetLevelManager().PlayTextAudio("EC-1");
            }


            if (SceneManager.GetActiveScene().name != "Level 5")
            {
                await script.ResetLevel();
                spotted = false;
            }
            else
            {
                spotted = false;
                Debug.Log("ACTIVATE SWORD");
                sword.SetActive(true);
                fight = true;
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
            fight = false;
            sword.SetActive(false);
            Debug.Log("death");
            //player.transform.GetChild(0).gameObject.SetActive(false);
            death.Play();
            await GetLevelManager().PlayTextAudio("EC-2");
            gameObject.SetActive(false);
        }

        public Boolean IsFrozen()
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
        public void ResetPosition()
        {
            sword.SetActive(false);
            fight = false;
            health = 100;
            if(startingPosition!=null)
            gameObject.transform.position = startingPosition;
        }
    }
}