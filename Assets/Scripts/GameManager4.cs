using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SurgeonSim
{
    public class GameManager4 : MonoBehaviour
    {
        public float spawnSpeed = 1f;
        public bool introduceLevel = true;
        public GameObject player;
        public GameObject enemy;
        public EnemyConfig[] enemyConfigs;
        public Transform playerSpawn;
        public Transform enemySpawn;
        public int level = 2;
        public int trophyScore = 10000;
        public UIManager uiManager;

        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        SpeechIn speechIn;
        SpeechOut speechOut;
        int playerScore = 0;
        int enemyScore = 0;
        int gameScore = 0;
        float totalTime = 0;
        float levelStartTime = 0;

        private float targetTime = 25.0f;

        Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

        private GameObject scalpel;

        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();

            scalpel = GameObject.Find("Scalpel");

            /* if (level < 0 || level >= enemyConfigs.Length)
            {
                Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");
                level = 0;
            } */
        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();

            uiManager.UpdateUI(playerScore, enemyScore);

            Introduction();
        }

        void Update()
        {
            targetTime -= Time.deltaTime;

            if (targetTime <= 0)
            {
                speechOut.Speak("Time's up. Game Over.");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        async void Introduction()
        {
            // await speechOut.Speak("Welcome to the Surgeon Simulator Panto Edition");
            // TODO: 1. Introduce obstacles in level 2 (aka 1)
            await Task.Delay(1000);
            RegisterColliders();

            /*if (introduceLevel)
            {
                await IntroduceLevel();
            }*/

            // await speechOut.Speak("Introduction finished, game starts.");

            await ResetGame();
        }

        async Task IntroduceLevel()
        {
            await speechOut.Speak("There are six organs.");
            Level level = GetComponent<Level>();
            await level.PlayIntroduction();

            // TODO: 2. Explain enemy and player with weapons by wiggling and playing shooting sound

            // await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
            //string response = await speechIn.Listen(commands);
            // await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

            //if (response == "yes")
            //{
            //    await RoomExploration();
            //}
        }

        async Task RoomExploration()
        {
            /*while (true)
            {
                await speechOut.Speak("Say done when you're ready.");
                string response = await speechIn.Listen(commands);
                if (response == "done")
                {
                    return;
                }
            }*/
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

        async Task ResetGame()
        {
            await speechOut.Speak("spawning");
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("Say scalpel.");
            await speechIn.Listen(new Dictionary<string, KeyCode>() { { "scalpel", KeyCode.S } });
            scalpel.SetActive(true);

            await speechOut.Speak("Reach the heart. Remember: Don't cut in an organ. You got twenty-five seconds.");
            GameObject kidney = GameObject.Find("Heart");
            await lowerHandle.SwitchTo(kidney, 0.3f);

            upperHandle.Free();

            player.SetActive(true);
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

        public async void OnDefeat(GameObject defeated)
        {
            player.SetActive(false);
            enemy.SetActive(false);

            bool playerDefeated = defeated.Equals(player);

            if (playerDefeated)
            {
                enemyScore++;
            }
            else
            {
                playerScore++;
            }
            uiManager.UpdateUI(playerScore, enemyScore);

            string defeatedPerson = playerDefeated ? "You" : "Enemy";
            await speechOut.Speak($"{defeatedPerson} got defeated.");

            gameScore += CalculateGameScore(player, enemy);

            level++;
            if (level >= enemyConfigs.Length)
            {
                await GameOver();
            }
            else
            {
                // TODO: Evaluate the players performance with game score
                await speechOut.Speak($"Current score is {gameScore}");
                await speechOut.Speak($"Continuing with level {level + 1}");
                await ResetGame();
            }
        }

        async Task GameOver()
        {
            await speechOut.Speak("Congratulations.");

            if (!GetComponent<DualPantoSync>().debug)
            {
                await speechOut.Speak($"You achieved a score of {gameScore}.");
                await speechOut.Speak("Please enter your name to submit your highscore.");

                await uiManager.GameOver(gameScore, (int)totalTime, trophyScore);
            }
            else
            {
                await speechOut.Speak($"You achieved a score of {gameScore} in debug mode.");
            }

            await speechOut.Speak("Thanks for playing DuelPanto. Say quit when you're done.");
            await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

            Application.Quit();
        }

        int CalculateGameScore(GameObject player, GameObject enemy)
        {
            Health playerHealth = player.GetComponent<Health>();
            Health enemyHealth = enemy.GetComponent<Health>();

            float levelCompleteTime = Time.time - levelStartTime;
            totalTime += levelCompleteTime;
            int timeMultiplier = 1;
            if (levelCompleteTime < 30)
            {
                timeMultiplier = 5;
            }
            else if (levelCompleteTime < 45)
            {
                timeMultiplier = 3;
            }
            else if (levelCompleteTime < 60)
            {
                timeMultiplier = 2;
            }

            int levelScore = playerHealth.healthPoints - enemyHealth.healthPoints;
            if (levelScore > 0)
            {
                int levelMultiplier = (int)(Mathf.Pow(2, level) + 1);
                levelScore *= timeMultiplier * levelMultiplier;
            }

            return levelScore;
        }
    }
}
