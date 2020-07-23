using Stealth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stealth
{
    public class SwordController : MonoBehaviour
    {
        public AudioSource hit;
        [System.Serializable]
        public enum Targets { Player, Enemy };
        public Targets targetType;
        string target;
        // Start is called before the first frame update
        void Start()
        {
            target = Enum.GetName(typeof(Targets), targetType);

        }

        // Update is called once per frame
        void Update()
        {

        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("This collision is " + other.gameObject.tag);

            if (other.gameObject.tag == target)
            {

                hit.Play();
                if (target == "Enemy")
                {
                    other.gameObject.GetComponent<EnemyController>().TakeHit();
                }
                else if (target == "Player")
                {
                    Debug.Log("Col with player");
                    other.gameObject.GetComponent<PlayerController>().TakeHit();
                }

            }
        }
    }
}