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
    public Transform enemySpawn;
    public int level = 0;
    public int trophyScore = 10000;

    public GameObject atom1, atom2;
    public Transform playerSpawnPosition;
    public PrimitiveType bond;

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
        await speechOut.Speak("Welcome to Project EduChem");
        // TODO: 1. Introduce obstacles in level 2 (aka 1)
        await Task.Delay(1000);
        RegisterColliders();

        //if (introduceLevel){
            await IntroduceLevel();
        //}

        await FirstLevel();
        await FeelForYourself();

        SecondLevel(); //starts a new scene
        Application.Quit();

        //await ResetGame();
    }

    async Task IntroduceLevel()
    {
        //await speechOut.Speak("There are two obstacles.");
        Debug.Log("Introduction");
        Level level = GetComponent<Level>();
        if (introduceLevel) await level.PlayIntroduction(); //says "these are the two atoms, make the bond"
    }

    async Task FirstLevel() {

        GameObject[] atoms = GameObject.FindGameObjectsWithTag("Atom");
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

        await speechOut.Speak("Well Done");
        cylinder.transform.position = new Vector3(cylinder.transform.position.x, cylinder.transform.position.y-0.5f, atom1.transform.position.z);
        level = 1;

        cylinder.AddComponent<BoxCollider>();
        cylinder.AddComponent<PantoBoxCollider>();

        BoxCollider box = cylinder.GetComponent<BoxCollider>();
        box.transform.position = cylinder.transform.position;
        box.transform.localScale = cylinder.transform.localScale;

        PantoBoxCollider pantoBox = cylinder.GetComponent<PantoBoxCollider>();
        pantoBox.transform.position = cylinder.transform.position;
        pantoBox.transform.localScale = cylinder.transform.localScale;
        await Task.Delay(1000);

        RegisterColliders();

    }

    void SecondLevel()
    {
        Debug.Log("second Level start");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");

        //await IntroduceLevel();
        //await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        //await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });
    }

    async Task FeelForYourself() { 

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

        if (playerDefeated)
        {
            enemyScore++;
        }
        else
        {
            playerScore++;
        }

        string defeatedPerson = playerDefeated ? "You" : "Enemy";
        await speechOut.Speak($"{defeatedPerson} got defeated.");

        gameScore += CalculateGameScore(player, enemy);

        level++;
        if (level >= enemyConfigs.Length)
        {
            await GameOver();
        } else
        {
            // TODO: Evaluate the players performance with game score
            await speechOut.Speak($"Current score is {gameScore}");
            await speechOut.Speak($"Continuing with level {level + 1}");
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

        if (!GetComponent<DualPantoSync>().debug)
        {
            await speechOut.Speak($"You achieved a score of {gameScore}.");
            await speechOut.Speak("Please enter your name to submit your highscore.");

        } else
        {
            await speechOut.Speak($"You achieved a score of {gameScore} in debug mode.");
        }

        await speechOut.Speak("Thanks for playing DuelPanto. Say quit when you're done.");
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
