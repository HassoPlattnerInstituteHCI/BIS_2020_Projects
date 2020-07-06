using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itembox : MonoBehaviour
{
    public GameObject mushroom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gotHit()
    {
        Instantiate(mushroom, transform.position+ new Vector3(0,1,0), transform.rotation);
    }
}
