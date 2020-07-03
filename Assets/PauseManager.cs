using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool isPaused { get; private set; } = false;
    public bool wasPaused { get; private set; } = true;

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
