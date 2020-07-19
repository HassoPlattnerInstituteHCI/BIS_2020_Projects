using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    [RequireComponent(typeof(MultiLevel))]
    public class EnableTrigger : MonoBehaviour
    {
        public GameObject[] enableAfterIntro;

        void Start()
        {
            GetComponent<MultiLevel>().IntroductionFinished += OnIntroductionFinished;
        }

        void OnIntroductionFinished(object sender)
        {
            foreach (GameObject g in enableAfterIntro)
            {
                g.SetActive(true);
            }
        }
    }
}