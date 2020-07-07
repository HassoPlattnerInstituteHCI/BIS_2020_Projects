using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;


namespace MarioKart
{
    public class MeHandleRep : MonoBehaviour
    {
        PantoHandle upperHandle;

        // Start is called before the first frame update
        void Start()
        {
            upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(-8, 0, upperHandle.HandlePosition(transform.position).z);
            transform.eulerAngles = new Vector3(0, upperHandle.GetRotation(), 0);
        }
    }
}