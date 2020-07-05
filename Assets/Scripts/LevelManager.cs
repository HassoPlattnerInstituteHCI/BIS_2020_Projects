using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Stealth
{
    public class LevelManager : MonoBehaviour
    {
        public float spawnSpeed = 1f;
        public bool introduceLevel = true;
        public GameObject player;
        public GameObject[] enemies;
        public GameObject currentEnemy;
        public Transform playerSpawn;
        public Transform[] enemySpawns;
        public UpperHandle upperHandle;
        public LowerHandle lowerHandle;
        public SpeechIn speechIn;
        public SpeechOut speechOut;
        public Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D },
            {"switch",KeyCode.E }
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
            await speechOut.Speak("Welcome to Quake Panto Edition");
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
            await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

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
        public async Task ResetGame()
        {
            Debug.Log("Fail");
            //await speechOut.Speak("Fail");
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