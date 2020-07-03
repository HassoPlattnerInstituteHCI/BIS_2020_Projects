using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;

namespace MarioKart
{
    public class Tutorial : MonoBehaviour
    {
        public MultiLevel[] levels;
        public int levelIndex;
        private SpeechOut speech;
        public bool autoStart = true;

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

        void OnApplicationQuit()
        {
            speech.Stop();
        }
    }
}