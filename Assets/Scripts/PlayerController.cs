using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Rider.Editor;
using SpeechIO;
using UnityEngine;

namespace Stealth
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody playerRb;
        public float speed = 1.0f;
        private PantoHandle upperHandle;
        public GameObject sword;
        public bool frozen;
        private AudioListener audioListener;
        public AudioSource StepsLeftAudioSource;
        SpeechOut speechOut;
        public AudioSource StepsRightAudioSource;

        private bool leftStep = true;
        private Vector3 lastStepPosition;
        public float health = 100.0f;

        public float HitPower = 10.0f;

        // Start is called before the first frame update
        void Start()
        {
            playerRb = GetComponent<Rigidbody>();
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            audioListener = GetComponent<AudioListener>();
            speechOut = new SpeechOut();
        }

        public void StartFighting()
        {
            sword.GetComponent<Collider>().enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (isFrozen())
            {
                return;
            }

            transform.position = getUpperHandle().HandlePosition(transform.position);
            transform.rotation = Quaternion.Euler(0, upperHandle.GetRotation(), 0);
            StepSound();
        }

        public Boolean isFrozen()
        {
            return frozen;
        }

        public void Freeze()
        {
            getUpperHandle().MoveToPosition(transform.position, 0.3f, false); // freeze handle
            frozen = true;
            // gameObject.SetActive(false);
            // getAudioListener().enabled = true;
        }

        public void Unfreeze()
        {
            frozen = false;
            gameObject.SetActive(true);
            upperHandle.Free();
        }

        private PantoHandle getUpperHandle()
        {
            if (upperHandle == null)
            {
                upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            }

            return upperHandle;
        }

        private AudioListener getAudioListener()
        {
            if (audioListener == null)
            {
                audioListener = GetComponent<AudioListener>();
            }

            return audioListener;
        }

        private void StepSound()
        {
            if (lastStepPosition == null
                || Vector3.Distance(lastStepPosition, gameObject.transform.position) > 1.0f)
            {
                if (leftStep)
                {
                    StepsLeftAudioSource.Play();
                }
                else
                {
                    StepsRightAudioSource.Play();
                }
                lastStepPosition = gameObject.transform.position;
                leftStep = !leftStep;
            }
        }
        public void TakeHit()
        {
            if (health > 0)
            {
                health = health - HitPower;
                Debug.Log("Player health is " + health);
            }

            if (health == 0)
            {
                PlayerDeath();
            }
        }
        async void PlayerDeath()
        {
            LevelManager script = GameObject.Find("Panto").GetComponent<LevelManager>();
            script.FreezeGameObjects();
            await speechOut.Speak("You are dead. Try again.");
            //sword.SetActive(false);
            health = 100;
            await script.ResetLevel();
            
            
        }
    }
}