using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DualPantoFramework;

namespace eduChem
{
    public class GameManager : MonoBehaviour
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
        public PrimitiveType bond;

        public GameObject[] objects;

        public AudioClip success;

        [HideInInspector]
        public GameObject playerSpawn;
        public AudioSource audioSource;

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
            audioSource = GetComponent<AudioSource>();

            Introduction();
        }

        async void Introduction()
        {
            await speechOut.Speak("Welcome to Project EduChem");
            await Task.Delay(1000);
            RegisterColliders();

            await IntroduceLevel();
            await FirstLevel();
            await FeelForYourself();

            await Quiz();

            SecondLevel(); //starts a new scene
            Application.Quit();
        }

        async Task IntroduceLevel()
        {
            //await speechOut.Speak("There are two obstacles.");
            Debug.Log("Introduction");
            Level level = GetComponent<Level>();
            if (introduceLevel) await level.PlayIntroduction(); //says "these are the two atoms, make the bond"
        }

        async Task Quiz()
        {
            await speechOut.Speak("Here is a little quiz for you. Is this an actual molecule? Reminder: These are 2 carbon atoms connected with one bond."+
                "Say yes or no.");
            string response = await speechIn.Listen(new Dictionary<string, KeyCode> { { "yes", KeyCode.Y }, { "no", KeyCode.N } });
            if (response == "yes")
            {
                await speechOut.Speak("You are wrong. That was really hard! C2, the molecule made of two carbon atoms, has a double bond. We will learn more about this in a moment.");
            } else if (response == "no")
            {
                await speechOut.Speak("Yes. thats correct!");
            }
        }

        async Task FirstLevel()
        {
            await speechOut.Speak("They should be connected with a chemical bond. " +
                "Create this bond by saying start bond, moving the handle to the other atom und say end bond.");

            GameObject[] atoms = GameObject.FindGameObjectsWithTag("Carbon Atom");
            if (atoms[0].transform.position.x < atoms[1].transform.position.x) //left atom is atom1
            {
                GameObject atom1 = atoms[0];
                GameObject atom2 = atoms[1];
            }
            else
            {
                GameObject atom1 = atoms[1];
                GameObject atom2 = atoms[0];
            }

            await upperHandle.SwitchTo(playerSpawn, 0.3f);
            await Task.Delay(500);
            upperHandle.Free();

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            bool doneLvl1 = false;
            while (!doneLvl1)
            {

                await speechIn.Listen(new Dictionary<string, KeyCode> { { "start bond", KeyCode.S } });

                Vector3 pos1 = upperHandle.GetPosition();
                cylinder.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                cylinder.transform.position = pos1;
                cylinder.transform.rotation = Quaternion.Euler(0, 0, 90);

                await speechIn.Listen(new Dictionary<string, KeyCode> { { "end bond", KeyCode.E } });

                Vector3 pos2 = upperHandle.GetPosition();
                cylinder.transform.localScale = new Vector3(2.5f, System.Math.Abs(pos2.x - pos1.x) / 2 + 0.3f, 0.5f);
                cylinder.transform.position = new Vector3(System.Math.Abs(pos1.x + pos2.x) / 2 - 0.2f, pos1.y, pos1.z);

                //im sorry for that terrible style, gonna change that later

                if (cylinder.transform.position.z < atom1.transform.position.z + 1 && cylinder.transform.position.y > atom1.transform.position.z - 1)
                {
                    if ((cylinder.transform.position.x + 0.5f * cylinder.transform.localScale.x) <= atom1.transform.position.x + atom1.transform.localScale.x)
                    {
                        if ((cylinder.transform.position.x + 0.5f * cylinder.transform.localScale.x) >= atom2.transform.position.x - atom2.transform.localScale.x)
                        {
                            doneLvl1 = true;
                        }
                        else
                        {
                            await speechOut.Speak("Try again");
                        }
                    }
                    else
                    {
                        await speechOut.Speak("Try again");
                    }
                }
                else
                {
                    await speechOut.Speak("Try again");
                }
            }

            audioSource.PlayOneShot(success);
            await speechOut.Speak("Well Done");
            cylinder.transform.position = new Vector3(atoms[0].transform.position.x + atoms[1].transform.position.x, cylinder.transform.position.y - 0.5f, atom1.transform.position.z);
            cylinder.transform.localScale = new Vector3(2.5f, 1, 0.5f);
            CapsuleCollider cap = cylinder.GetComponent<CapsuleCollider>();
            Object.Destroy(cap);
            cylinder.tag = "Single Bond";
            BoxCollider box = cylinder.AddComponent<BoxCollider>();
            PantoBoxCollider pantoBox = cylinder.AddComponent<PantoBoxCollider>();

            objects[2] = cylinder;

            await Task.Delay(1000);

            RegisterColliders();

        }

        void SecondLevel()
        {
            Debug.Log("second Level start");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");
        }

        async Task FeelForYourself()
        {
            await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");

            bool exploring = true;

            while (exploring) { 
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