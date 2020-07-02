using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool isLeft = false;
    private int leftBlock;
    private int rightBlock;
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
        Instantiate(groups[leftBlock], transform.position, transform.rotation);
    } else {Instantiate(groups[rightBlock], transform.position, transform.rotation);}
    }
}
