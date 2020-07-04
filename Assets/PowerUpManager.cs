﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using SpeechIO;

namespace MarioKart
{
    // Accepts a powerup and allows using it
    public class PowerUpManager : MonoBehaviour
    {
        private Player player;
        private SpeechOut speechOut;
        private SpeechIn speechIn;
        private Powerup.PowerupType activePowerup = Powerup.PowerupType.None;

        // Start is called before the first frame update
        void Start()
        {
            player = GetComponent<Player>();
            speechOut = new SpeechOut();
            speechIn = new SpeechIn(OnSpeechRecognized);
            speechIn.StartListening(new string[] { "description", "use" });
        }

        void OnApplicationQuit()
        {
            speechOut.Stop();
            speechIn.StopListening();
        }

        // Give a powerup. If it already has one, say so and discard the new one.
        public void GivePowerup(Powerup.PowerupType type, bool doAnounce)
        {
            if (activePowerup != Powerup.PowerupType.None)
            {
                Say("You already have a powerup!");
                return;
            }

            activePowerup = type;

            if (!doAnounce)
            {
                return;
            }

            switch (type)
            {
                case Powerup.PowerupType.Booster:
                    Say("You got a booster!");
                    break;

                case Powerup.PowerupType.Shockwave:
                    Say("You got a shockwave!");
                    break;

                case Powerup.PowerupType.Laser:
                    Say("You got a laser!");
                    break;
                default:
                    Say("You got a programming error!");
                    break;
            }
        }

        public delegate void OnSayFinished(object sender);
        public event OnSayFinished SayFinished;
        private async void Say(string message)
        {
            await speechOut.Speak(message);
            SayFinished?.Invoke(this);
        }

        void OnSpeechRecognized(string command)
        {
            print("Recoglized command " + command);
            if (command == "description")
            {
                print("Saying description!");
                SayDescription();
            }
            else if (command == "use")
            {
                StartCoroutine(UsePowerup());
            }
        }

        void OnDescriptionSayFinished(object sender)
        {
            SaidDescription?.Invoke(this, activePowerup);
        }

        public void SayDescription()
        {
            switch (activePowerup)
            {
                case Powerup.PowerupType.None:
                    Say("You do not have a powerup!");
                    break;

                case Powerup.PowerupType.Booster:
                    Say("You have a booster. You can use it to get a speed Boost for 2 Seconds");
                    break;

                case Powerup.PowerupType.Shockwave:
                    Say("You have a shockwave. You can use it to stun opponents around you");
                    break;

                case Powerup.PowerupType.Laser:
                    Say("You have a laser. You can use it to stun next opponent in front of you");
                    break;
            }
            SayFinished += OnDescriptionSayFinished;
        }

        public IEnumerator UsePowerup()
        {
            switch (activePowerup)
            {
                case Powerup.PowerupType.None:
                    Say("You do not have a powerup!");
                    break;
                //Booster
                case Powerup.PowerupType.Booster:
                    Say("You used a booster!");
                    player.speed = 20.0f;
                    yield return new WaitForSeconds(2);
                    player.speed = player.defaultSpeed;
                    break;

                    //Shockwave
                    /* braucht noch Enemy
                    case 2:
                        if (Vector3.Distance(GameObject.FindObjectOfType<Player>().transform.position, GameObject.FindObjectOfType<Enemy>().transform.position) < 2)
                        {
                            GameObject.FindObjectOfType<Enemy>().SetSpeed(0.0f);
                            yield return new WaitForSeconds(5);
                            GameObject.FindObjectOfType<Enemy>().SetSpeed(10.0f);
                        }

                        break;

                    //Laser
                    case 3: 
                        GameObject.FindObjectOfType<Enemy>().SetSpeed(0.0f);
                        yield return new WaitForSeconds(3);
                        GameObject.FindObjectOfType<Enemy>().SetSpeed(10.0f);
                        break;
                    */
            }
            UsedPowerup?.Invoke(this, activePowerup);
            activePowerup = Powerup.PowerupType.None;
        }

        public delegate void OnUsedPowerup(object sender, Powerup.PowerupType powerup);
        public delegate void OnSaidDescription(object sender, Powerup.PowerupType powerup);

        public event OnUsedPowerup UsedPowerup;
        public event OnSaidDescription SaidDescription;
    }
}