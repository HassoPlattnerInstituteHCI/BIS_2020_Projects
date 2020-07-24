using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using SpeechIO;
using UnityEditor.PackageManager.Requests;

namespace MarioKart
{
    public class Goal : MonoBehaviour
    {
        private float distance = 5;
        private bool lap = true;
        private int lapcount = 0;
        public PathCreator pathCreator;
        public int MaxLaps;
        public GameObject checkpoint;
        public int place = 1;
        private float time = 0;
        private SpeechOut speechOut;
        private SpeechIn speechIn;
        public GameObject player;
        public GameObject enemy;

        private void Start()
        {
            speechOut = new SpeechOut();
        }
        void Update()
        {
            time += Time.deltaTime;
        }

        void OnTriggerEnter(Collider other)
        {
            SetPauseAll(true);
            if (other.CompareTag("Player"))
            {
                place = 1;
                Victory();
            }
            else if (other.CompareTag("Enemy"))
            {
                place = 2;
                Victory();
            }

        }

        async void Victory()
        {
            //nächstes level laden/ siegerehrung?
            if (place == 1)
            {
                await speechOut.Speak("Congratulations you won. This is the end of the Demo.");
            }
            else
            {
                await speechOut.Speak("The Enemy was faster this time");
            }

            // speechIn = new SpeechIn(OnSpeechRecognized);
            // speechIn.StartListening(new string[] { "stop" });
        }

        //         void OnSpeechRecognized(string command)
        //         {
        //             print("Recoglized command " + command);
        //             if (command == "stop")
        //             {
        //                 print("Quitting application...");
        // #if UNITY_EDITOR
        //                 UnityEditor.EditorApplication.isPlaying = false;
        // #else
        // UnityEngine.Application.Quit();
        // #endif
        //             }
        //         }

        void Reset()
        {
            player.GetComponent<DraggedPlayer>().Reset();
            enemy.GetComponent<Enemy>().Reset();
            player.GetComponent<PowerUpManager>().Reset();
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

        void OnApplicationQuit()
        {
            speechOut.Stop();
            // speechIn.StopListening();
        }
    }
}
