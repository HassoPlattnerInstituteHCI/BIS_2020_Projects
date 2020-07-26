using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DualPantoFramework;

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

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    float totalTime = 0;
    float levelStartTime = 0;
    public Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D },
        { "weapon one", KeyCode.Alpha1 },
        { "weapon two", KeyCode.Alpha2 },
        { "weapon three", KeyCode.Alpha3 }
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
        await speechOut.Speak("Welcome to C o D Blackout Panto Edition");
        // TODO: 1. Introduce obstacles in level 2 (aka 1)
        await Task.Delay(1000);

        if (introduceLevel)
        {
            await IntroduceLevel();
        }

        await speechOut.Speak("Introduction finished, game starts.");

        await ResetGame();
    }

    async Task IntroduceLevel()
    {
        upperHandle.Free();
        lowerHandle.Free();
        await GetComponent<Levels>().PlayIntroduction(level,speechIn);
        RegisterColliders();
        upperHandle.Free();
        lowerHandle.Free();
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

    void RegisterColliders() {
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
        player.transform.rotation = playerSpawn.rotation;
        await upperHandle.SwitchTo(player, 0.3f);
        /*if (level == 0)                                                                                 //LINO HELLIGE
        {
            await upperHandle.SwitchTo(GameObject.Find("Player Spawn"), 0.3f);
        }*/
        await speechOut.Speak("Spawning enemy");
        enemy.transform.position = enemySpawn.position;
        enemy.transform.rotation = enemySpawn.rotation;
        if(level < 2)
        {
            await lowerHandle.SwitchTo(enemy, 0.3f);
        }
        if (level >= enemyConfigs.Length)
            Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");
        enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

        if(level == 0)
        {
            if(GetComponent<DualPantoSync>().debug)
            {
                upperHandle.FreeRotation();
            }
            else
            {
                upperHandle.Free();
            }

        }
        else
        {
            upperHandle.Free();
        }
        if (level <= 4)
        {
            GetComponent<Levels>().GunListener(speechIn);
        }

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

    /// <summary>
    /// Ends the round by disabling player and enemy, updating UI, calculating
    /// game score and eventually ending the game.
    /// </summary>
    /// <param name="defeated"></param>
    public async void OnDefeat(GameObject defeated)
    {
        player.SetActive(false);
        enemy.SetActive(false);

        bool playerDefeated = defeated.Equals(player);

        string defeatedPerson = playerDefeated ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");

        level++;
        if (level >= enemyConfigs.Length)
        {
            await GameOver();
        } else
        {
            // TODO: Evaluate the players performance with game score
            await IntroduceLevel();
            await ResetGame();
        }
    }

    /// <summary>
    /// Ends the game.
    /// </summary>
    /// <returns></returns>
    async Task GameOver()
    {
        await speechOut.Speak("Congratulations.");

        await speechOut.Speak("Thanks for playing C o D Blackout. Say quit when you're done.");
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }

    /// <summary>
    /// Calculates the game score by healt, level complete time and enemy
    /// difficulty.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
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
        } else if (levelCompleteTime < 45)
        {
            timeMultiplier = 3;
        } else if (levelCompleteTime < 60)
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
