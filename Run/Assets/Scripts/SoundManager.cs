using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioCollision;
    public AudioClip audioDie;
    public AudioClip audioCoin;
    public AudioClip audioLevelCom;
    public AudioClip audioWalk;
    public AudioClip audioItembox;
    public AudioClip audioMushroom;
    public void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Walk":
                audioSource.clip = audioWalk;
                break;
            case "Attack":
                audioSource.clip = audioAttack;
                break;
            case "Damaged":
                audioSource.clip = audioCollision;
                break;
            case "Coin":
                audioSource.clip = audioCoin;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "Finish":
                audioSource.clip = audioLevelCom;
                break;
            case "Itembox":
                audioSource.clip = audioItembox;
                break;
            case "Mushroom":
                audioSource.clip = audioMushroom;
                break;
        }
        audioSource.Play();
    }
}
