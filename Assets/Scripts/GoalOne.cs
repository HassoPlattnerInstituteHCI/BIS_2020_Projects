using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    public class GoalOne : LevelTrigger
    {
        private SpeechIO.SpeechOut speechOut;

        void Start()
        {
            speechOut = new SpeechIO.SpeechOut();
        }

        void OnApplicationQuit()
        {
            speechOut.Stop();
        }

        async void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetPauseAll(true);
                await speechOut.Speak("Nicely done!");
                SetPauseAll(true);
            }
            else if (other.CompareTag("Enemy"))
            {
                SetPauseAll(true);
                await speechOut.Speak("Too bad, you will get them next time!");
                SetPauseAll(true);
            }
            Activate();
        }

        void SetPauseAll(bool isPaused)
        {
            PauseManager[] pauseManagers = GameObject.FindObjectsOfType<PauseManager>();
            foreach (PauseManager p in pauseManagers)
            {
                if (isPaused)
                {
                    p.Pause();
                }
                else
                {
                    p.Unpause();
                }
            }
        }
    }
}