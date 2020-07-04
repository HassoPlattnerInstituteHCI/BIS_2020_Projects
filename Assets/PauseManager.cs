using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    public class PauseManager : MonoBehaviour
    {
        public bool isPaused = false;
        public bool wasPaused { get; private set; } = false;

        void Start()
        {
            wasPaused = !isPaused;
        }

        void Update()
        {
            wasPaused = isPaused;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Unpause()
        {
            if (isPaused)
            {
                wasPaused = true;
            }
            isPaused = false;
        }
    }
}