using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DualPantoFramework;


public class BatLogic : MonoBehaviour
{
    private PantoHandle lowerHandle;

    private PantoHandle upperHandle;

    GameObject bat;
    GameObject player;

    private PlayerSoundEffect playerSounds;



    AudioSource audioSource;



    GameManager gameManager;



    void Start()
    {
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();

        bat = GameObject.Find("Bat");


        audioSource = GetComponent<AudioSource>();

        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }

    void Update()
    {

        //transform.position = player.transform.position;
        // Simply connects the bat to the lower handles position

        transform.position = upperHandle.HandlePosition(transform.position);

        Vector3 playerPos = upperHandle.HandlePosition(transform.position);

        transform.rotation = Quaternion.Euler(0, upperHandle.GetRotation(), 0);

        Vector3 playerDirection = transform.forward;
        float spawnDistance = 1.5f;

        Vector3 batPos = playerPos + playerDirection*spawnDistance;

        transform.position = batPos;

        //transform.RotateAround(player.transform.position, Vector3.up, 20 * Time.deltaTime);


    }


    //TODO: Rotation hinkriegen

    //TODO: Level 2 Story

    void OnTriggerEnter(Collider collider1)
    {
        if(collider1.CompareTag("dangerous")){
            player = GameObject.Find("Player");
            playerSounds = player.GetComponent<PlayerSoundEffect>();
            playerSounds.playObjectHitByBat();
        }
        if(collider1.CompareTag("AHole")){
            player = GameObject.Find("Player");
            playerSounds = player.GetComponent<PlayerSoundEffect>();
            playerSounds.playAHoleHitByBat();
            gameManager.spawnAHoles(1);
            gameManager.hitCount++;
            Debug.Log("hitcount: " + gameManager.hitCount);

            //Calculate Cash/Game Score
            if(gameManager.hitCount < 5) gameManager.cash++;
            else if(gameManager.hitCount < 10) gameManager.cash+=2;
            else if(gameManager.hitCount < 30) gameManager.cash+=5;
            else gameManager.cash+=10;
            Debug.Log("Cash: " + gameManager.cash);

            
            if(gameManager.currentLevel == 4){
                gameManager.currentObjectiveReached = true;
            }

        }

        if(collider1.CompareTag("Cop")){
            GameObject cop = collider1.gameObject;
            CopSoundEffect copSounds = cop.GetComponent<CopSoundEffect>();
            copSounds.playCopHit();

        }

    }

    void OnTriggerExit(Collider collider1){
        if(collider1.CompareTag("AHole")){
            gameManager.deleteAHole(collider1.gameObject);
        }
        if(collider1.CompareTag("Cop")){
            //reduce his health
            //collider1.gameObject.healthLeft -= 1;
            collider1.GetComponent<CopLogic>().healthLeft -= 1;
            Debug.Log("should reduce cop health");
            //CopLogic cop = collider1.GetComponent<Mover>();
        }
    }
}
