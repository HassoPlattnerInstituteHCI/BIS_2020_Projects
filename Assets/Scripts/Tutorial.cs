﻿using System.Collections;
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

        public bool GetTutorial()
        {
            return tutorial;
        }
    }
}