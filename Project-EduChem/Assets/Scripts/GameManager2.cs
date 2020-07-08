using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DualPantoFramework;

namespace eduChem
{

    public class GameManager2 : MonoBehaviour
    {
        public float spawnSpeed = 1f;
        public bool introduceLevel = true;
        public GameObject player;
        public GameObject enemy;
        public EnemyConfig[] enemyConfigs;
        public Transform enemySpawn;
        public int level = 0;
        public int trophyScore = 10000;

        public GameObject atom1, atom2;
        public Transform playerSpawnPosition;
        public GameObject[] objects;

        [HideInInspector]
        public GameObject playerSpawn;

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
            { "done", KeyCode.D },
            {"explain", KeyCode.E },
            {"show", KeyCode.S },
            {"quit", KeyCode.Q }
    };

        void Awake()
        {
            speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
            speechOut = new SpeechOut();

            if (level < 0 || level >= enemyConfigs.Length)
            {
                Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");
                level = 0;
            }
        }

        void Start()
        {
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();

            playerSpawn = new GameObject();
            playerSpawn.transform.position = playerSpawnPosition.position;

            //objects = GetComponents<GameObject>();

            Introduction();
        }

        async void Introduction()
        {
            RegisterColliders();
            await speechOut.Speak("Level2 started");
            await IntroduceLevel();

            await SecondLevel();

            await Quiz();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level3");
            Application.Quit();
        }

        async Task IntroduceLevel()
        {
            //await speechOut.Speak("There are two obstacles.");
            Debug.Log("Introduction");
            Level level0 = GetComponent<Level>();
            if (introduceLevel) await level0.PlayIntroduction(); //says "these are the two atoms, make the bond"
        }

        async Task SecondLevel()
        {
            await FeelForYourself();
        }

        async Task FeelForYourself()
        {

            await speechOut.Speak("Feel for yourself again. You can use two new commands too: Say Show to restart the introduction to this level to get a better orientation." +
                "Or say explain while touching an object to get to know what kind of atom or object you are touching. Say yes or done when you're ready.");

            bool exploring = true;

            while (exploring)
            {
                string response = await speechIn.Listen(commands);

                switch (response)
                {
                    case "show":
                        await IntroduceLevel();
                        break;
                    case "explain":
                        await Explain();
                        break;
                    case "quit":
                        Application.Quit();
                        break;
                    case "yes":
                    case "done":
                        exploring = false;
                        break;
                }
            }
        }

        async Task Quiz() //TODO new question
        {
            await speechOut.Speak("Here is a little quiz for you. Is this an actual molecule? Reminder: These are 2 carbon atoms connected with one bond." +
                "Say yes or no.");
            string response = await speechIn.Listen(new Dictionary<string, KeyCode> { { "yes", KeyCode.Y }, { "no", KeyCode.N } });
            if (response == "yes")
            {
                await speechOut.Speak("You are wrong. That was really hard! C2, the molecule made of two carbon atoms, has a double bond.");
            }
            else if (response == "no")
            {
                await speechOut.Speak("Yes. thats correct!");
            }
        }

        async Task Explain()
        {
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = upperHandle.GetPosition();
            GameObject nearest = null;

            foreach (GameObject obj in objects)
            {
                Vector3 dist = obj.transform.position - currentPosition;
                float distSqr = dist.sqrMagnitude;
                if (distSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distSqr;
                    nearest = obj;
                }
            }

            await speechOut.Speak("This is a " + nearest.tag);
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

        void onRecognized(string message)
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