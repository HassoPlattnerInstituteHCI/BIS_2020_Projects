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
            OnPauseChanged.Invoke(isPaused);
        }

        void Update()
        {
            wasPaused = isPaused;
        }

        public void Pause()
        {
            if (!isPaused)
            {
                OnPauseChanged.Invoke(true);
            }
            isPaused = true;
        }

        public void Unpause()
        {
            if (isPaused)
            {
                wasPaused = true;
                OnPauseChanged.Invoke(false);
            }
            isPaused = false;
        }

        public delegate void PauseChangedHandler(bool isPaused);
        public event PauseChangedHandler OnPauseChanged;
    }
}