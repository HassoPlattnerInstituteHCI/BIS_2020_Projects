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
        public void NextCheckpoint()
        {
            if (pathCreator.path.length-2 <= distance)
            {
                lap = true;
            }
            else
            {
                distance += 1;
                Instantiate(checkpoint, pathCreator.path.GetPointAtDistance(distance), pathCreator.path.GetRotationAtDistance(distance));
            }
        }

        public void NextLap()
        {
            if (lap)
            {
                lapcount++;
                if (lapcount == MaxLaps + 1)
                {
                    Victory();
                }
                else
                {
                    distance = 2;
                    lap = false;
                    NextCheckpoint();
                }             
            }
        }

        public int getMaxLaps()
        {
            return MaxLaps;
        }

        public void increasePlace()
        {
            place++;
        }
        async void Victory()
        {
            Destroy(gameObject);
            //nächstes level laden/ siegerehrung?
            if (place == 1)
            {
                await speechOut.Speak("Congratulations you won. This is the end of the Demo, but you can redo the track by saying 'YES'.");
            }
            else
            {
                await speechOut.Speak("The Enemy was faster this time. If you want to retry this track say 'YES'");
            }

            speechIn = new SpeechIn(OnSpeechRecognized);
            speechIn.StartListening(new string[] { "YES" });
        }

        void OnSpeechRecognized(string command)
        {
            print("Recoglized command " + command);
            if (command == "YES")
            {
                Reset();
            }
        }

        void Reset()
        {
            lapcount = 0;
            time = 0;
            place = 1;
            //StartSound
            player.GetComponent<Player>().Spawn();
            enemy.GetComponent<Enemy>().Spawn();
            player.GetComponent<PowerUpManager>().Reset();
        }

        void OnApplicationQuit()
        {
            speechOut.Stop();
            speechIn.StopListening();
        }
    }
}
