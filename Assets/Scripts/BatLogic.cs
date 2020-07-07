using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BatLogic : MonoBehaviour
{
    private PantoHandle lowerHandle;

    private PantoHandle upperHandle;

    GameObject bat;
    GameObject player;



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
        player = GameObject.Find("Player");
        //transform.position = player.transform.position;
        // Simply connects the bat to the lower handles position

        transform.position = upperHandle.HandlePosition(transform.position);

        Vector3 playerPos = upperHandle.HandlePosition(transform.position);

        transform.rotation = Quaternion.Euler(0, upperHandle.GetRotation(), 0);

        Vector3 playerDirection = transform.forward;
        float spawnDistance = 1; 
        
        Vector3 batPos = playerPos + playerDirection*spawnDistance; 

        transform.position = batPos;                
        
    }

    void OnTriggerEnter(Collider collider1)
    {   
        
    }
}