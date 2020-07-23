using System;
using System.Collections;
using System.Collections.Generic;
using Stealth;
using UnityEngine;

namespace Stealth
{
    public class ObstacleController : MonoBehaviour
    {
        public AudioSource hitAudioSource;
        private LevelManager _script;
        // Start is called before the first frame update
        void Start()
        {
        }

        private LevelManager GetScript()
        {
            if (_script == null)
            {
                _script = GameObject.Find("Panto").GetComponent<LevelManager>();
            }

            return _script;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision with obstacle");
            hitAudioSource.Play();
            GetScript().OnObstacleHit();
        }
    }
}