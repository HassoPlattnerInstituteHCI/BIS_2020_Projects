using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    public class Powerup : MonoBehaviour
    {
        public enum PowerupType
        {
            None,
            Booster,
            Shockwave,
            Laser,
        };

        public PowerupType powerupType = PowerupType.Booster;
        public bool doAnouncePickup = true;
        void OnTriggerEnter(Collider other)
        {
            PowerUpManager powerUpManager;
            if (other.TryGetComponent<PowerUpManager>(out powerUpManager))
            {
                powerUpManager.GivePowerup(powerupType, doAnouncePickup);
                GameObject.Destroy(gameObject);
            }
        }
    }
}