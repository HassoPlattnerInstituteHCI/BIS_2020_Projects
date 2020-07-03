using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private int leftBlock;
    private int rightBlock;
    public static bool spawnWavePls = true;
    public static int waveNumber = 0;
    public static GameObject blockLeft;
    public static GameObject blockRight;
    public GameObject[] groups;

    // Start is called before the first frame update
    void Start()
    {
        //spawnNext();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnWavePls) {
            spawnNext();
            spawnWavePls = false;
        }
    }

    public void spawnNext() {
        leftBlock = Random.Range(0, groups.Length);
        blockLeft = Instantiate(groups[leftBlock], transform.position, transform.rotation);
        blockLeft.name = "LeftBlock";
        rightBlock = Random.Range(1, groups.Length);
        if (rightBlock==leftBlock) {
            rightBlock = 0;
        }
        blockRight = Instantiate(groups[rightBlock], transform.position + new Vector3 ((float)2.5, 0, 0), transform.rotation);
        blockRight.name = "RightBlock";
        waveNumber++;
    }
}
