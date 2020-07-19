using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace MarioKart
{
    public class FollowMe : MonoBehaviour
    {
        public LowerHandle handle;
        // Start is called before the first frame update
        void Start()
        {
            GameObject panto = GameObject.FindGameObjectWithTag("Panto");
            print(panto);
            handle = panto.GetComponent<LowerHandle>();
        }

        // Update is called once per frame
        void Update()
        {
            handle.SwitchTo(gameObject, 0.2f);
        }
    }
}