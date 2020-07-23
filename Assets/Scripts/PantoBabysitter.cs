using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;

namespace MarioKart
{
    public class PantoBabysitter : MonoBehaviour
    {
        // The ObstacleManager decided to take some time off and has left us to clean up after him

        async void Start()
        {
            await Task.Delay(2000);
            RegisterColliders();
        }

        void RegisterColliders()
        {
            PantoCollider[] colliders = GameObject.FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider c in colliders)
            {
                Debug.Log("Creating collider " + c.gameObject.name + " because ObstacleManager sucks");
                c.CreateObstacle();
                c.Disable();
                c.Enable();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RegisterColliders();
            }
        }
    }

}