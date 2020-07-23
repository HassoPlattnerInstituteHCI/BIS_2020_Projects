using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;

namespace MarioKart
{
    public class PausableItHandle : MonoBehaviour
    {
        private PauseManager pauseManager;
        private PantoHandle handle;
        // Start is called before the first frame update
        async void Start()
        {
            handle = GameObject.Find("Panto").GetComponent<LowerHandle>();
            pauseManager = GetComponent<PauseManager>();
            pauseManager.OnPauseChanged += OnPauseChangedHandler;

            await Task.Delay(2000);
            if (!pauseManager.isPaused)
            {
                CaptureHandle();
            }

        }

        void OnPauseChangedHandler(bool isPaused)
        {
            if (!isPaused)
            {
                CaptureHandle();
            }
        }

        async void CaptureHandle()
        {
            await handle.SwitchTo(gameObject, 0.2f);
        }
    }
}