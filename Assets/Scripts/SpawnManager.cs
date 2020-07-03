﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool isLeft = false;
    private int leftBlock;
    private int rightBlock;
    public static GameObject blockLeft;
    public static GameObject blockRight;
    public GameObject[] groups;

    // Start is called before the first frame update
    void Start()
    {
        spawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnNext() {

    leftBlock = Random.Range(0, groups.Length);
    rightBlock = Random.Range(1, groups.Length);
    if (rightBlock==leftBlock) {
        rightBlock = 0;
    }
    // Spawn Group at current Position
    if(isLeft) {
        blockLeft = Instantiate(groups[leftBlock], transform.position + new Vector3 (0, 1, 0), transform.rotation);
        blockLeft.name = "LeftBlock";
        
    } else {
        blockRight = Instantiate(groups[rightBlock], transform.position + new Vector3 (0, 1, 0), transform.rotation);
        blockRight.name = "RightBlock";
        }
    }
}
