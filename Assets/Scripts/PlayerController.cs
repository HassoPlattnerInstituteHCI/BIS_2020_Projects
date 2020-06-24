using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 1.0f; 

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        PantoMovement();
    }
    
    void PantoMovement()
        {
            float rotation = GameObject
                .Find("Panto")
                .GetComponent<UpperHandle>()
                .getRotation();
            Vector3 direction = Quaternion.Euler(0, rotation, 0) * Vector3.forward;
            playerRb.velocity = speed * direction;
        }
}
