using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using DualPantoFramework;
using System.Collections;

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

    public AudioSource audioSourceMusic;

    public AudioClip hitZeroMusic;

    public AudioClip hit4TransitionMusic;

    public AudioClip killingStreakMusic;

    public AudioClip copTurnsCash;
    //public AudioSource rollSource;

    //private GameObject TelephoneBox1;

    //public SpeechOut speechOut = new SpeechOut();
    private bool transitionMusicIsPlaying = false;

    private bool transitionMusicHasBeenPlayed = false;
    GameManager gameManager;

    void Start()
    {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
        
        speechOut = new SpeechOut();
        //We dont need to get the audiosources here because we assign them directly in unity as its otherwise complicates with multiple sources
        //audioSource = GetComponents<AudioSource>();
        //audioSourcePolice = gameObject.AddComponent<AudioSource>();
    }

    void Update(){
        if(audioSourcePolice.isPlaying){
            //May improve this to make setting by seconds possible
            audioSourcePolice.volume = audioSourcePolice.volume + 0.0005f;
        }

        if(transitionMusicIsPlaying == false && gameManager.hitCount == 4 && transitionMusicHasBeenPlayed == false){
            Debug.Log("StartHit4Music gets calöled gets called");
            startHit4TransitionMusic();
        }

        if(transitionMusicIsPlaying == true && audioSourceMusic.isPlaying == false){
            transitionMusicIsPlaying = false;
            Debug.Log("StartKillingStreakMusic gets called");
            startKillingStreakMusic();
            StartCoroutine(gameManager.makeWaveOfCopsArriveAfterTime(15, 1));
            if(gameManager.currentLevel == 5){
                gameManager.currentObjectiveReached = true;
            }
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
        audioSourcePolice.volume = 0.02f;
        audioSourcePolice.clip = sirens;
        audioSourcePolice.Play();
        
    }

    public void startHitZeroMusic(){
        //audioSource = GetComponent<AudioSource>();
        transitionMusicIsPlaying = false;
        transitionMusicHasBeenPlayed = false;
        audioSourceMusic.loop = true;
        audioSourceMusic.volume = 0.03f;
        audioSourceMusic.clip = hitZeroMusic;
        audioSourceMusic.Play();
        
    }

    public void startHit4TransitionMusic(){
        //audioSource = GetComponent<AudioSource>();
        audioSourceMusic.Stop();
        audioSourceMusic.loop = false;
        audioSourceMusic.volume = 0.05f;
        audioSourceMusic.clip = hit4TransitionMusic;
        audioSourceMusic.Play();
        transitionMusicIsPlaying = true;
        transitionMusicHasBeenPlayed = true;

        
    }

    public void startKillingStreakMusic(){
        audioSourceMusic.Stop();
        audioSourceMusic.loop = true;
        audioSourceMusic.volume = 0.05f;
        audioSourceMusic.clip = killingStreakMusic;
        audioSourceMusic.Play();
    }

    
    public void playCopTurnsCash(){
        audioSource.PlayOneShot(copTurnsCash);
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
