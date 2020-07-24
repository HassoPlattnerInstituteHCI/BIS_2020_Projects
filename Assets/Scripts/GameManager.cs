using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using DualPantoFramework;


namespace PantoGolf
{
    public class GameManager : MonoBehaviour
    {
        public bool introduceLevel = true;
        public GameObject Player;
        static public int level = 0;
        public int levelHitCount = 0;
        static public int totalHitCount = 0;

        UpperHandle upperHandle;
        LowerHandle lowerHandle;
        SpeechIn speechIn;
        SpeechOut speechOut;
        Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "reset", KeyCode.R },
        { "gamereset", KeyCode.G }
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
            Player = GameObject.Find("Player");
            Player.GetComponent<PlayerScript>().allowMovement = false;
            //Player.SetActive(false);
            Introduction();
            speechIn.StartListening();
        }

        async void Introduction()
        {
            //await lowerHandle.SwitchTo(GameObject.Find("Ball"), 0.2f);
            if (SceneManager.GetActiveScene().name == "Level 1")
            {
                await speechOut.Speak("Welcome to PantoGolf.");
            }
            await speechOut.Speak(SceneManager.GetActiveScene().name);  //Announce current level
                                                                        //Introduce Level to player:
            if (introduceLevel)
            {
                await IntroduceLevel();
            }

            // Set IT Handle to follow the ball
            await lowerHandle.SwitchTo(GameObject.Find("Ball"), 0.1f);
            await (upperHandle).SwitchTo(Player, 0.1f);
            upperHandle.Free();
            Player.GetComponent<PlayerScript>().allowMovement = true;
            //Player.SetActive(true);
            Debug.Log("Introduction finished, game starts.");
            //await speechOut.Speak("Introduction finished, game starts.");
            //await lowerHandle.SwitchTo(GameObject.Find("Ball"), 0.2f);
            //await ResetGame();
        }

        async Task IntroduceLevel()
        {
            if (GameObject.FindGameObjectsWithTag("obstacle").Length > 4)
            {
                await speechOut.Speak("There are " + (GameObject.FindGameObjectsWithTag("obstacle").Length - 4) + " obstacles.");
            }
            //await speechOut.Speak("There are no obstacles.");
            Level level = GetComponent<Level>();
            await level.PlayIntroduction();

            //string response = await speechIn.Listen(commands);

            //if (response == "yes")
            //{
            //    await RoomExploration();
            //}
        }

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

        /*void RegisterColliders() {
            PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
            foreach (PantoCollider collider in colliders)
            {
                Debug.Log(collider);
                collider.CreateObstacle();
                collider.Enable();
            }
        } */

        async Task ResetGame()
        {
            Debug.Log("Restarting the game");
            await speechOut.Speak("Restarting the game.");
            level = 0;
            LoadScene(level);
        }

        public void LevelComplete()
        {
            Debug.Log("You completed the level.");
            level++;
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));
            }
            else
            {
                LastLevelComplete();
            }
        }

        public void RestartLevel()
        {
            Debug.Log("Restarting the level.");
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        async void onRecognized(string message)
        {
            Debug.Log("SpeechIn recognized: " + message);
            switch (message)
            {
                case "reset":
                    RestartLevel();
                    break;
                case "gamereset":
                    await ResetGame();
                    break;
                default:
                    Debug.Log("No procedure for command " + message);
                    break;
            }
        }

        public void OnApplicationQuit()
        {
            speechOut.Stop(); //Windows: do not remove this line.
            speechIn.StopListening(); // [macOS] do not delete this line!
        }

        async Task GameOver()
        {
            await speechOut.Speak("Thanks for playing PantoGolf.");
            Application.Quit();
        }

        async Task LastLevelComplete()
        {
            await speechOut.Speak("You completed all levels of PantoGolf! Congratulations");
            Application.Quit();
        }

        private void LoadScene(int index)
        {
            Debug.Log("Try to load Scene with index " + index);
            SceneManager.LoadScene(index);
        }
    }
}
