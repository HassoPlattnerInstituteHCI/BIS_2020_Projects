using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace dualLayouting
{
    public class WallScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Created Obstacle for a Wall");
            GetComponent<PantoCollider>().CreateObstacle();
            GetComponent<PantoCollider>().Enable();
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}