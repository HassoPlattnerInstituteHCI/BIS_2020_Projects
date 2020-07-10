using UnityEngine;
using SpeechIO;

public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip wasted;
    public AudioClip objectHit;
    //a lot of different sounds for hitting passengers:)
    public AudioClip aHoleHit;
    public AudioClip aHole1;
    public AudioClip aHole2;
    public AudioClip aHole3;
    public AudioClip aHole4;
    public AudioClip aHole5;
    public AudioClip aHole6;
    public AudioClip aHole7;
    public AudioClip aHole8;
//end of passenger hitting
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

    void Update(){
        if(audioSourcePolice.isPlaying){
            //May improve this to make setting by seconds possible
            audioSourcePolice.volume = audioSourcePolice.volume + 0.0002f;
        }
        
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

    public void playAHoleHitByBat(){
        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(aHoleHit);
        System.Random r = new System.Random();
        int rInt = r.Next(1, 8);
        AudioClip unique = aHole1;
        //beautiful long line that makes sure that one of the 8 Clips get played
        if(rInt==1) unique = aHole1; else if(rInt==2) unique = aHole2; else if(rInt==3) unique = aHole3; else if(rInt==4) unique = aHole4; else if(rInt==5) unique = aHole5; else if(rInt==6) unique = aHole6; else if(rInt==7) unique = aHole7; else if(rInt==8) unique = aHole8;
        audioSource.PlayOneShot(unique);
    }
    public void startSirens(){
        //audioSource = GetComponent<AudioSource>();
        audioSourcePolice.loop = true;
        audioSourcePolice.volume = 0.1f;
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
