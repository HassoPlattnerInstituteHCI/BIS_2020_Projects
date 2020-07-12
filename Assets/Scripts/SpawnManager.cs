using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using DualPantoFramework;

namespace Tetris {
public class SpawnManager : MonoBehaviour
{
    public static int leftBlock;
    public static int rightBlock;
    public static bool spawnIntroPls = false;
    public static int introCounter = 0;
    public static bool spawnWavePls = false;
    public static int waveNumber = 0;
    public static GameObject blockLeft;
    public static GameObject blockRight;
    public GameObject[] skylines;
    public GameObject[] groups;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnWavePls) {
            spawnNext();
        }
        if(spawnIntroPls) {
            spawnIntro(introCounter);
        }
    }

    //spawns the next two blocks, one on the position of the Spawner(Left), the other offset. 
    //Sets their names accordingly, as well as tags for the children (might delete that)
    public void spawnNext() {
        leftBlock = Random.Range(0, groups.Length);
        blockLeft = Instantiate(groups[leftBlock], transform.position, transform.rotation);
        blockLeft.name = ""+leftBlock;
        Player.leftBlockRootPos = blockLeft.transform.GetChild(0).transform.position;
        rightBlock = Random.Range(1, groups.Length);
        if (rightBlock==leftBlock) {
            rightBlock = 0;
        }
        blockRight = Instantiate(groups[rightBlock], transform.position + new Vector3 ((float)2.5, 0, 0), transform.rotation);
        blockRight.name = ""+rightBlock;
        Player.rightBlockRootPos = blockRight.transform.GetChild(0).transform.position;
        waveNumber++;
        spawnWavePls = false;
    }

    //Custom spawn function for the Intro-levels.
    public void spawnIntro(int level) {
        switch(level) {
            case 0: leftBlock = 3; //Picks the yellow block, which fits in the gap
                    Instantiate(skylines[level], transform.position + new Vector3((float)-0.5, 0, (float)-7), transform.rotation);
                    Playfield.confirmBlock(GameObject.Find("IntroSkyline"+level+"(Clone)"));
                    blockLeft = Instantiate(groups[leftBlock], transform.position, transform.rotation);
                    blockLeft.name = ""+leftBlock;
                    Player.leftBlockRootPos = blockLeft.transform.GetChild(0).transform.position;
                    spawnIntroPls = false;
                    return; //Level 0 is special, since we only spawn one block
            case 1: leftBlock = 0;
                    rightBlock = 6; //forces player to switch blocks
                break;
            case 2: leftBlock = 0;
                    rightBlock = 5;
                break;
            case 3: leftBlock = 2;
                    rightBlock = 4;
                break;
            case 4: Instantiate(skylines[level], transform.position + new Vector3(-0.5f, 0, (float)-7), transform.rotation);
                    Playfield.confirmBlock(GameObject.Find("IntroSkyline"+(level-1)+"(Clone)"));
                    spawnIntroPls=false;
                    spawnWavePls=true;
                return;
        }
        Instantiate(skylines[level], transform.position + new Vector3(-0.5f, 0, (float)-7), transform.rotation);
        Playfield.confirmBlock(GameObject.Find("IntroSkyline"+level+"(Clone)"));
        blockLeft = Instantiate(groups[leftBlock], transform.position, transform.rotation);
        blockLeft.name = ""+leftBlock;
        Player.leftBlockRootPos = blockLeft.transform.GetChild(0).transform.position;
        blockRight = Instantiate(groups[rightBlock], transform.position + new Vector3 ((float)2.5, 0, 0), transform.rotation);
        blockRight.name = ""+rightBlock;
        Player.rightBlockRootPos = blockRight.transform.GetChild(0).transform.position;
        spawnIntroPls = false;
    }
}
}