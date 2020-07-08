using UnityEngine;
using SpeechIO;

public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip wasted;
    public AudioClip objectHit;
    public AudioClip sirens;

    SpeechOut speechOut;

    public AudioSource audioSource;
    //public AudioSource rollSource;

    //private GameObject TelephoneBox1;

    //public SpeechOut speechOut = new SpeechOut();

    void Start()
    {

        speechOut = new SpeechOut();
        //audioSource = GetComponent<AudioSource>();
        //rollSource = gameObject.AddComponent<AudioSource>();
        //rollSource.loop = true;
        //rollSource.clip = rollClip;
    }

    public void playWasted(){

        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = wasted;
        audioSource.Play();
    }

    public void playObjectHitByBat(){
        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = objectHit;
        audioSource.Play();

    }
    public void startSirens(){
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = sirens;
        audioSource.Play();
        
    }
    public void StopPlayback()
    {
        audioSource.Stop();
    }
}
