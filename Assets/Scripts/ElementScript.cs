﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dualLayouting 
{


    public class ElementScript : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponentInChildren<TextMesh>().text = name;
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
