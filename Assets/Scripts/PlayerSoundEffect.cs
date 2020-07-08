using UnityEngine;
using SpeechIO;

public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip wasted;
    public AudioClip objectHit;
    public AudioClip sirens;

    SpeechOut speechOut;

    public AudioSource audioSource;
    public AudioSource audioSourcePolice;
    //public AudioSource rollSource;

    //private GameObject TelephoneBox1;

    //public SpeechOut speechOut = new SpeechOut();

    void Start()
    {

        speechOut = new SpeechOut();
        //audioSource = GetComponents<AudioSource>();
        audioSourcePolice = gameObject.AddComponent<AudioSource>();
        //rollSource.loop = true;
        //rollSource.clip = rollClip;
    }

    public void playWasted(){

        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(wasted);
    }

    public void playObjectHitByBat(){
        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(objectHit);

    }
    public void startSirens(){
        //audioSource = GetComponent<AudioSource>();
        audioSourcePolice.loop = true;
        audioSourcePolice.clip = sirens;
        audioSourcePolice.Play();
        
    }
    public void StopPlayback()
    {
        audioSource.Stop();
    }
    public void StopPolicePlayback()
    {
        audioSourcePolice.Stop();
    }
}
