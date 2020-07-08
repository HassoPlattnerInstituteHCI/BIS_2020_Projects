using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour
{

    GameObject player;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("MeHandle");

        Debug.Log("start atom script");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Debug.Log("on collision");

        if(other.tag == "Player")
        {
            audioSource.Play();
            Debug.Log("play");
        }
    }
}
