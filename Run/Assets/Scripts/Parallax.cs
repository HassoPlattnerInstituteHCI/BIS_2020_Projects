using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Code from this YT Video: https://www.youtube.com/watch?v=zit45k6CUMk&t=20s


    private float length, startpos;
    public GameObject camera;
    public float parallaxAmount;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = (camera.transform.position.x * (1 - parallaxAmount));
        float distance = (camera.transform.position.x * parallaxAmount);
        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }


}
