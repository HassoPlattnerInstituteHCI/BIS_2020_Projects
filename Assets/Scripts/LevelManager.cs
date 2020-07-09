﻿using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stealth
{
    public abstract class LevelManager : MonoBehaviour
    {
        public float spawnSpeed = 1f;
        public bool introduceLevel = true;
        public GameObject player;
        public GameObject[] enemies;
        public GameObject currentEnemy;
        public int EnemyIndex = 0;
        public Transform playerSpawn;
        public Transform[] enemySpawns;
        public UpperHandle upperHandle;
        public LowerHandle lowerHandle;
        public SpeechIn speechIn;
        public SpeechOut speechOut;

        public Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>()
        {
            {"yes", KeyCode.Y},
            {"no", KeyCode.N},
            {"done", KeyCode.D},
            {"switch", KeyCode.E}
        };

        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();
        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();

            Introduction();
        }

        async void Introduction()
        {
            await speechOut.Speak("Welcome to Stealth Panto");
            // TODO: 1. Introduce obstacles in level 2 (aka 1)
            await Task.Delay(1000);
            RegisterColliders();

            if (introduceLevel)
            {
                await IntroduceLevel();
                await speechOut.Speak("Introduction finished, game starts.");
            }

            await ResetGame();
        }

        async Task IntroduceLevel()
        {
            await speechOut.Speak("There are two obstacles.");
            Level level = GetComponent<Level>();
            await level.PlayIntroduction();

            await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
            //string response = await speechIn.Listen(commands);
            await speechIn.Listen(new Dictionary<string, KeyCode>() {{"yes", KeyCode.Y}, {"done", KeyCode.D}});

            //if (response == "yes")
            //{
            //    await RoomExploration();
            //}
        }


        void RegisterColliders()
        {
            PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in colliders)
            {
                Debug.Log(collider);
                collider.CreateObstacle();
                collider.Enable();
            }
        }

        /// <summary>
        /// Starts a new round.
        /// </summary>
        /// <returns></returns>
        abstract public Task ResetGame();

        /// <summary>
        /// Complete the level.
        /// Supposed for cleanup and redirecting to the next level.
        /// </summary>
        /// <returns></returns>
        abstract public Task Success();


        async void onRecognized(string message)
        {
            Debug.Log("SpeechIn recognized: " + message);
        }

        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.
            speechIn.StopListening(); // [macOS] do not delete this line!
        }

        public async Task ListenToSwitch()
        {
            string response = await speechIn.Listen(commands);
            if (response == "switch")
            {
                ToggleEnemies();
                await lowerHandle.SwitchTo(currentEnemy, 0.3f);
            }

            ListenToSwitch();
        }

        private void ToggleEnemies()
        {
            if (EnemyIndex < enemies.Length - 1)
            {
                EnemyIndex++;
            }
            else EnemyIndex = 0;

            currentEnemy = enemies[EnemyIndex];
        }

        public void DeactivateGameObjects()
        {
            player.SetActive(false);
            foreach (GameObject en in enemies)
            {
                en.SetActive(false);
                en.GetComponent<EnemyController>().canSpot = false;
            }
        }

        public void ActivateGameObjects()
        {
            player.SetActive(true);
            foreach (GameObject en in enemies)
            {
                en.SetActive(true);
                en.GetComponent<EnemyController>().canSpot = true;
            }
        }

        public async Task SpawnPlayer()
        {
            await speechOut.Speak("Spawning player");
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);
        }

        public async Task SpawnEnemies()
        {
            await speechOut.Speak("Spawning enemies");

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = enemySpawns[i].position;
                enemies[i].transform.rotation = enemySpawns[i].rotation;
            }

            EnemyIndex = 0;
            if (enemies.Length > 0)
            {
                currentEnemy = enemies[EnemyIndex];
                await lowerHandle.SwitchTo(currentEnemy, 0.3f);
            }
        }
    }
}