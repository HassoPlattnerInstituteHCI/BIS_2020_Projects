using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

public class PlayerLogic : MonoBehaviour
{
    private PantoHandle upperHandle;

    AudioSource audioSource;
    public AudioClip heartbeatClip;
    public float timeLeft = 4;
    public bool countdown = false;
    public int startBPM = 60;
    public int endBPM = 220;
    float bpmCoefficient;
    public float bps = 1;
    float nextHeartbeat;
    Health health;

    GameManager gameManager;

    private TelephoneSoundEffect telephoneSounds;
    private PlayerSoundEffect playerSounds;
    GameObject phoneBox;

    private CopSoundEffect copSounds;

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        phoneBox = GameObject.Find("TelephoneBox1");

        telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();

        playerSounds = GetComponent<PlayerSoundEffect>();


        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2);

        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }

    void Update()
    {
        // Simply connects the player to the upper handles position
        transform.position = upperHandle.HandlePosition(transform.position);

        if(countdown) timeLeft -= Time.deltaTime;
        if(timeLeft <= 0) playerDies();

        if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
        {
            if (nextHeartbeat > bps)
            {
                float bpm = bpmCoefficient * Mathf.Pow(health.healthPoints - health.maxHealth, 2) + startBPM;
                bps = 60f / bpm;
                //audioSource.PlayOneShot(heartbeatClip);
                nextHeartbeat = 0;
            }
            else
            {
                nextHeartbeat += Time.deltaTime;
            }
        }
    }

    void OnTriggerEnter(Collider collider1)
    {   
        if(gameManager.currentLevel==1){
            if(collider1.CompareTag("TelephoneBox1")){           
                telephoneSounds.StopPlayback();
                telephoneSounds.startPhoneTalks();
            }
        }
        if(gameManager.currentLevel==2){
            if(collider1.CompareTag("TelephoneBox2")){         
                phoneBox = GameObject.Find("TelephoneBox2");
                telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();  
                telephoneSounds.StopPlayback();
                telephoneSounds.startPhoneTalks();
            }
        }
        if(gameManager.currentLevel==3){
            if(collider1.CompareTag("safehouse")){
                playerSounds.StopPolicePlayback();
                telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();  
                telephoneSounds.StopPlayback();
                telephoneSounds.startPhoneTalks();
            }
        }
        if(gameManager.currentLevel == 4 && gameManager.currentObjectiveReached){
            Debug.Log("OnTrigger Enter for Level 4 called");
            if(collider1.CompareTag("safehouse")){      
                GameObject phoneBox = GameObject.Find("TelephoneBox2");
                TelephoneSoundEffect telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();  
                telephoneSounds.StopPlayback();
                telephoneSounds.startPhoneTalks();
            }           
        }

        
        if((gameManager.currentLevel == 5 && gameManager.currentObjectiveReached) || gameManager.currentLevel == 6){
            Debug.Log("OnTrigger Enter for Level 5 called");
            if(collider1.CompareTag("safehouse")){      
                GameObject phoneBox = GameObject.Find("TelephoneBox2");
                TelephoneSoundEffect telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();  
                telephoneSounds.StopPlayback();
                telephoneSounds.startPhoneTalks();
            }           
        }
            

        if(collider1.CompareTag("dangerous")){   //player should die when running into an obstacle
            playerDies();
        } 
        if(collider1.CompareTag("safehouse")){
            playerSounds.StopPolicePlayback();
            playerSounds.startHitZeroMusic();
            Debug.Log("hitcount: " + gameManager.hitCount);
            Debug.Log("cops killed: " + gameManager.copsKilled);
            Debug.Log("cash: " + gameManager.cash);
            gameManager.hitCount = 0;
            gameManager.copsKilled = 0;
            //police should disappear
            GameObject[] cops = GameObject.FindGameObjectsWithTag("Cop");
            foreach(GameObject cop in cops){
                GameObject.Destroy(cop);
            }
                

        }
        if(collider1.CompareTag("Cop")){   //player should die when staying in the radius of the cop
            countdown = true;

            GameObject cop = collider1.gameObject;
            copSounds = cop.GetComponent<CopSoundEffect>();
            copSounds.stopRadio();
            copSounds.playShout();

        } 
    }

    void OnTriggerExit(Collider collider1){
        if(collider1.CompareTag("Cop")){   
            resetTimer();

            Debug.Log("OnTriggerLeaveWasCalled");

            GameObject cop = collider1.gameObject;
            copSounds = cop.GetComponent<CopSoundEffect>();
            copSounds.StopPlayback();
            copSounds.startCopsRadioTalk();
        } 
    }

    public void playerDies(){
        resetTimer();
        playerSounds.playWasted();
        gameManager.currentObjectiveReached = false;
        playerSounds.StopPolicePlayback();
        playerSounds.startHitZeroMusic();
        gameManager.ResetGame();
    }

    public void resetTimer(){
        countdown = false;
        timeLeft = 4;
    }
}
