using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager3 : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool introduceLevel = true;
    public GameObject player;
    public GameObject[] enemies;
    private GameObject enemy;
    public EnemyConfig[] enemyConfigs;
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
    private int EnemyIndex = 0;
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D },
        {"switch",KeyCode.E }
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
        } else EnemyIndex = 0;
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

        await speechOut.Speak("Spawning enemies");
        enemy.transform.position = enemySpawn.position;
        enemy.transform.rotation = enemySpawn.rotation;
        await lowerHandle.SwitchTo(enemy, 0.3f);
        await speechOut.Speak("Follow the ticking sound and find treasure avoiding enemies. Say Switch to switch between enemies.");
        if (level >= enemyConfigs.Length)
            Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");
        //enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

        upperHandle.Free();

        player.SetActive(true);
        foreach(GameObject en in enemies)
        {
            en.SetActive(true);
        }
        levelStartTime = Time.time;
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
