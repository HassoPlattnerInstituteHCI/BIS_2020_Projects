﻿using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace eduChem {

    public class GameManager3 : MonoBehaviour
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
        { "start bond", KeyCode.S },
        { "end bond", KeyCode.E }
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

            Introduction();
        }

        async void Introduction()
        {
            // await speechOut.Speak("Welcome to Project EduChem");
            // TODO: 1. Introduce obstacles in level 2 (aka 1)
            // await Task.Delay(1000);
            RegisterColliders();

            //if (introduceLevel){
            await speechOut.Speak("Level 3 started");
            await IntroduceLevel();
            //}

            //await FirstLevel();
            //await FeelForYourself();

            await FeelForYourself();
            await ThirdLevel();

            UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");
            Application.Quit();
        }

        async Task IntroduceLevel()
        {
            await speechOut.Speak("Now we want to take that a step further. There are both the carbon atoms. But there are just 4 of the 6 hydrogen atoms from the ethane molecule.");
            //Debug.Log("Introduction");
            //Level level0 = GetComponent<Level>();
            //if (introduceLevel) await level0.PlayIntroduction();
            //await speechOut.Speak("Congratulations, you created your first molecule: ethene.");
        }

        async Task ThirdLevel()
        {
            await speechOut.Speak("Now we want to create ethene. It's structure is very similar to the structure of ethane. But instead of 6 hydrogen atoms there are just 4 left. That causes the electrons that were " +
                "part of the bonds to 2 missing hydrogen atoms to build a second bond between the two carbon atoms. Now its your turn to create this double bond.");
            await createBond();
            await speechOut.Speak("This is the first bond.");
            await createBond();
            await speechOut.Speak("Well done! You created a double bond and the resulting molecule is ethene.");



        }

        async Task createBond()
        {
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
                cylinder.transform.localScale = new Vector3(2.5f, System.Math.Abs(pos2.x - pos1.x) / 2 + 0.7f, 0.5f);
                cylinder.transform.position = new Vector3(System.Math.Abs(pos1.x + pos2.x) / 2 - 0.4f, pos1.y, pos1.z);

                //im sorry for that terrible style, gonna change later

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


            cylinder.transform.position = new Vector3(cylinder.transform.position.x, cylinder.transform.position.y - 0.5f, atom1.transform.position.z);
            level = 1;

            CapsuleCollider cap = cylinder.GetComponent<CapsuleCollider>();
            Object.Destroy(cap);
            BoxCollider box = cylinder.AddComponent<BoxCollider>();
            PantoBoxCollider pantoBox = cylinder.AddComponent<PantoBoxCollider>();
            await Task.Delay(1000);

            RegisterColliders();
        }
        async Task FeelForYourself()
        {

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
            //player.transform.position = playerSpawn.position;
            await upperHandle.SwitchTo(player, 0.3f);

            await speechOut.Speak("Spawning enemy");
            enemy.transform.position = enemySpawn.position;
            enemy.transform.rotation = enemySpawn.rotation;
            await lowerHandle.SwitchTo(enemy, 0.3f);
            if (level >= enemyConfigs.Length)
                Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");
            enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

            upperHandle.Free();

            player.SetActive(true);
            enemy.SetActive(true);
            levelStartTime = Time.time;
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
