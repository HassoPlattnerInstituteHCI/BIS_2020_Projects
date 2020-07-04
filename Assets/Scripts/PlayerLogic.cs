﻿using System.Collections;
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

        if (telephoneSounds == null)
        {
            Debug.LogError("No TelephoneSoundsEffect component found.");  
        }

        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2);
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
        Debug.LogError("OnTriggerEnter gets called"); 
        if(collider1.CompareTag("TelephoneBox1")){           

            telephoneSounds.StopPlayback();
            telephoneSounds.startPhoneTalks(1);
        }
        else if(collider1.CompareTag("dangerous")){//player should die when running into an obstacle
            playerSounds.playWasted();
            //reset game
        } 

    }
}
