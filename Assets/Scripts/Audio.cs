using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PantoDrawing
{

    public class Audio : MonoBehaviour
    {
        private AudioSource audioSource;
        public AudioClip drawing;
        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.pitch = 2;
            
        }

        
        public void drawingSound()
        {
            audioSource.clip = drawing;
            audioSource.Play();
        }

        public void stopSound()
        {
            audioSource.Stop();
        }

    }
}