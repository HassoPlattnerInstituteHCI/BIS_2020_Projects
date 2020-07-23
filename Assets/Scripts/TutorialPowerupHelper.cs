using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarioKart
{
    [RequireComponent(typeof(PowerUpManager))]
    public class TutorialPowerupHelper : MonoBehaviour
    {
        private PowerUpManager powerUpManager;
        public Tutorial tutorial;

        // Start is called before the first frame update
        void Start()
        {
            powerUpManager = GetComponent<PowerUpManager>();
            powerUpManager.UsedPowerup += OnUsedPowerup;
        }

        // Play final level (Level 4 end) and destroy this component
        async void OnUsedPowerup(object sender, Powerup.PowerupType powerup)
        {
            await tutorial.PlayNext();
            GetComponent<PowerUpManager>().SaidDescription -= OnUsedPowerup;
            Destroy(this);
        }
    }
}