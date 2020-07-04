using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using PathCreation;

namespace MarioKart
{
    public class Tutorial : MonoBehaviour
    {
        public MultiLevel[] levels;
        public int levelIndex;
        private SpeechOut speech;
        public bool autoStart = true;
        public PathCreator pathCreator;
        public GameObject checkpoint;
        private int checkpointnumber = 0;
        private bool tutorial = true;

        // Start is called before the first frame update
        async void Start()
        {
            speech = new SpeechOut();
            // GenerateNextCheckpoint();
            if (autoStart)
            {
                await PlayNext();
            }
        }

        void SetPause(bool isPaused)
        {
            PauseManager[] pausables = GameObject.FindObjectsOfType<PauseManager>();
            foreach (PauseManager p in pausables)
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

        public async Task PlayNext()
        {
            if (levelIndex < levels.Length)
            {
                MultiLevel level = levels[levelIndex++];
                SetPause(true);
                await speech.Speak(level.description);
                await level.PlayIntroduction();
                SetPause(false);
            }
        }

        public async void PlayLevel(int index)
        {
            levelIndex = index;
            await PlayNext();
        }

        void OnApplicationQuit()
        {
            speech.Stop();
        }

        async public void GenerateNextCheckpoint()
        {
            switch (checkpointnumber)
            {
                case 0:
                    Instantiate(checkpoint, pathCreator.path.GetPointAtDistance(50), pathCreator.path.GetRotationAtDistance(50));
                    checkpointnumber++;
                    break;

                case 1:
                    Instantiate(checkpoint, pathCreator.path.GetPointAtDistance(62), pathCreator.path.GetRotationAtDistance(62));
                    // GameObject.FindObjectOfType<PowerUpManager>().GenerateZone(60);
                    await speech.Speak("There is a powerup on the road, just continue driving to get it");
                    checkpointnumber++;
                    break;
                case 2:
                    await speech.Speak("Now that you got your powerup, say Description to find out what it does");
                    break;
            }
        }

        public bool GetTutorial()
        {
            return tutorial;
        }
    }
}