using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallAudio : MonoBehaviour
{

    public AudioClip[] WaterDrop;
    public AudioClip[] ClubHit;
    public AudioClip[] Goal;
    public AudioClip[] Rolling;
    public AudioClip[] Obstacle;
    public float maxPitch = 1.2f;
    public float minPitch = 0.8f;

    public AudioSource audioSource;
    private AudioSource rollingSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWaterDrop()
    {
        int i = Random.Range(0, WaterDrop.Length);
        PlayClipPitched(WaterDrop[i]);
    }

    public void PlayClubHit()
    {
        int i = Random.Range(0, ClubHit.Length);
        PlayClipPitched(ClubHit[i]);
    }

    public void PlayGoal()
    {
        int i = Random.Range(0, Goal.Length);
        PlayClipPitched(Goal[i]);
    }

    public void PlayObstacle()
    {
        int i = Random.Range(0, Obstacle.Length);
        PlayClipPitched(Obstacle[i]);
    }

    public void PlayRolling(float vol)
    {
        if (vol >= 1)
        {
            Debug.Log("Start Rolling sound");
            int i = Random.Range(0, Obstacle.Length);
            rollingSource.pitch = 1f;
            //rollingSource.PlayOneShot(Rolling[0]);
        }
    }

    public void StopRolling()
    {
        Debug.Log("End Rolling sound");
        //rollingSource.Stop();
    } 

    public void PlayClipPitched(AudioClip clip)
    {
        // little trick to make clip sound less redundant
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        // plays same clip only once, this way no overlapping
        audioSource.PlayOneShot(clip);
        audioSource.pitch = 1f;
    }
}
