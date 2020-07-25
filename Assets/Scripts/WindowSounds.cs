using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WindowSounds : MonoBehaviour
{
    public AudioClip witheSound;
    public AudioSource audioSource;

    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void playWindowClip()
    {
        PlayClipPitched(witheSound, minPitch, maxPitch);
    }
    public void PlayClipPitched(AudioClip clip, float minPitch, float maxPitch)
    {
        // little trick to make clip sound less redundant
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        audioSource.PlayOneShot(clip);
        audioSource.pitch = 1f;
    }
}
