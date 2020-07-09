using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Stealth
{
    public class LevelManager2 : LevelManager
    {
        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();
        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();

            //uiManager.UpdateUI(playerScore, enemyScore);

            Introduction();
        }

        async void Introduction()
        {
            await speechOut.Speak("Welcome to Stealth Panto Level 2");
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

            // TODO: 2. Explain enemy and player with weapons by wiggling and playing shooting sound

            await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
            //string response = await speechIn.Listen(commands);
            await speechIn.Listen(new Dictionary<string, KeyCode>() {{"yes", KeyCode.Y}, {"done", KeyCode.D}});

            //if (response == "yes")
            //{
            //    await RoomExploration();
            //}
        }

        [System.Obsolete]
        async Task RoomExploration()
        {
            while (true)
            {
                await speechOut.Speak("Say done when you're ready.");
                string response = await speechIn.Listen(commands);
                if (response == "done")
                {
                    return;
                }
            }
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
        override public async Task ResetGame()
        {
            // player.SetActive(true);
            // foreach (GameObject en in enemies)
            // {
            //    en.SetActive(true);
            //  }

            foreach (var enemy in enemies)
            {
                enemy.GetComponent<EnemyController>().canSpot = false;
            }

            await speechOut.Speak("Spawning player");
            Debug.Log("Player position: " + playerSpawn.position);
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("Spawning enemies");

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = enemySpawns[i].position;
                enemies[i].transform.rotation = enemySpawns[i].rotation;
                enemies[i].GetComponent<EnemyController>().canSpot = true;
            }

            currentEnemy = enemies[0];
            await lowerHandle.SwitchTo(currentEnemy, 0.3f);
            await speechOut.Speak(
                "An enemy protects the treasure. You can feel him using the it handle. Don't get to close to him.");

            //enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

            upperHandle.Free();

            player.SetActive(true);
            foreach (GameObject en in enemies)
            {
                en.SetActive(true);
            }
        }

        async void onRecognized(string message)
        {
            Debug.Log("SpeechIn recognized: " + message);
        }

        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.
            speechIn.StopListening(); // [macOS] do not delete this line!
        }

        async public Task Success()
        {
            SceneManager.LoadScene(sceneName: "Level 3");
        }
    }
}