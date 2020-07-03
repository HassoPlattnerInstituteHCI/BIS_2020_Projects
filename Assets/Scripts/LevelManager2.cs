using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stealth
{
    public class LevelManager2 : MonoBehaviour
    {
        public float spawnSpeed = 1f;
        public bool introduceLevel = true;
        public GameObject player;
        public GameObject enemy;
        public Transform playerSpawn;
        public Transform enemySpawn;
        public int level = 0;
        public int trophyScore = 10000;
        //public UIManager uiManager;

        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        SpeechIn speechIn;
        SpeechOut speechOut;
        int playerScore = 0;
        int enemyScore = 0;
        int gameScore = 0;
        float totalTime = 0;
        float levelStartTime = 0;
        Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
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

            //uiManager.UpdateUI(playerScore, enemyScore);

            Introduction();
        }

        async void Introduction()
        {
            await speechOut.Speak("Welcome to Quake Panto Edition");
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
        async Task ResetGame()
        {
            await speechOut.Speak("Spawning player");
            player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("Spawning enemy");
            enemy.transform.position = enemySpawn.position;
            enemy.transform.rotation = enemySpawn.rotation;
            await lowerHandle.SwitchTo(enemy, 0.3f);
            await speechOut.Speak("Follow the ticking sound and find treasure avoiding enemies.");
            
            //enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

            upperHandle.Free();

            player.SetActive(true);
            enemy.SetActive(true);
            levelStartTime = Time.time;
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