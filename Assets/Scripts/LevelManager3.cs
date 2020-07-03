using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stealth
{
    public class LevelManager3 : LevelManager
    {

       
        
        private int EnemyIndex = 0;
        

        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();

            
        }

        private async Task ListenToSwitch()
        {
            string response = await speechIn.Listen(commands);
            if (response == "switch")
            {
                ToggleEnemies();
                await lowerHandle.SwitchTo(enemy, 0.3f);

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
            enemy = enemies[EnemyIndex];

        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();
            enemy = enemies[0];

            //uiManager.UpdateUI(playerScore, enemyScore);

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
            }

            await speechOut.Speak("Introduction finished, game starts.");

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
            await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

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
        public async Task ResetGame()
        {
            player.SetActive(false);
            foreach (GameObject en in enemies)
            {
                en.SetActive(false);
            }
            await speechOut.Speak("Spawning player");
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("Spawning enemies");
            
           
            for(int i=0;i<enemies.Length; i++)
            {
                enemies[0].transform.position = enemySpawns[0].position;
                enemies[0].transform.rotation = enemySpawns[0].rotation;
            }
            await lowerHandle.SwitchTo(enemy, 0.3f);
            await speechOut.Speak("Follow the ticking sound and find treasure avoiding enemies. Say Switch to switch between enemies.");
           
            //enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

            upperHandle.Free();

            player.SetActive(true);
            foreach (GameObject en in enemies)
            {
                en.SetActive(true);
            }
            
            ListenToSwitch();
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



    }
}