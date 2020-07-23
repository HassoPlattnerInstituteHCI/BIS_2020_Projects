using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    // This class is used to trigger the next level in a tutorial. Use this for all triggers.
    public class LevelTrigger : MonoBehaviour
    {
        public Tutorial tutorial;
        public int levelIndex = 0;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Activate();
            }
        }

        protected void Activate()
        {
            print("Player has reached checkpoint " + name);
            tutorial.PlayLevel(levelIndex);
            GameObject.Destroy(gameObject);
        }
    }
}