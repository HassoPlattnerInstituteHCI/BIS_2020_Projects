using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace MarioKart
{
    public class SnapToMe : MonoBehaviour
    {
        // Start is called before the first frame update
        private PantoHandle handle;
        void Start()
        {
            handle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = handle.GetPosition();
        }
    }
}