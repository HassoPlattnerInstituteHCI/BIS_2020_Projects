using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool introduceLevel = true;
    public GameObject player;
    public GameObject enemy;
    public EnemyConfig[] enemyConfigs;
    public Transform playerSpawn;
    public Transform enemySpawn;
    public int level = 0;
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
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
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

        Introduction();
    }

    async void Introduction()
    {
        await lowerHandle.SwitchTo(GameObject.Find("Ball"), 0.2f);
        await speechOut.Speak("Welcome to Quake Panto Edition");
        // TODO: 1. Introduce obstacles in level 2 (aka 1)
        await Task.Delay(1000);
        //RegisterColliders();

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

        //string response = await speechIn.Listen(commands);
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

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
        upperHandle.Free();

        player.SetActive(true);
        levelStartTime = Time.time;
    }

    public void LevelComplete()
    {
        Debug.Log("You completed the level.");
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

    async Task GameOver()
    {
        await speechOut.Speak("Congratulations.");
        await speechOut.Speak("Thanks for playing PantoGolf. Say quit when you're done.");
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }
}
