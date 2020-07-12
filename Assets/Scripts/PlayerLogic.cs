using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private PantoHandle upperHandle;

    AudioSource audioSource;
    public AudioClip heartbeatClip;

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
            playerSounds.playWasted();
            gameManager.currentObjectiveReached = false;
            playerSounds.StopPolicePlayback();
            playerSounds.startHitZeroMusic();
            gameManager.ResetGame();
        } 
        if(collider1.CompareTag("safehouse")){
            gameManager.hitCount = 0;
            playerSounds.StopPolicePlayback();
            playerSounds.startHitZeroMusic();
            Debug.Log("hitcount: " + gameManager.hitCount);
            //police should disappear
        }
    }
}
