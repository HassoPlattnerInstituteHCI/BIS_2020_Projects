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

        uiManager.UpdateUI(playerScore, enemyScore);

        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to C o D Blackout Panto Edition");
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
        switch(level)
        {
            case 0:
                Level lvl = GetComponent<Level>();
                await lvl.PlayIntroduction();
                // TODO: 2. Explain enemy and player with weapons by wiggling and playing shooting sound                                                LINO HELLIGE
                await speechOut.Speak("Oh no there is an enemy");
                GameObject helpPos = new GameObject();
                helpPos.transform.position = enemySpawn.position;
                helpPos.transform.rotation = enemySpawn.rotation;
                lowerHandle = GetComponent<LowerHandle>();
                await lowerHandle.SwitchTo(helpPos, 0.5f);

                await speechOut.Speak("he is trying to kill you");
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(1, 0, 0), 0.1f);
                await Task.Delay(100);
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(-2, 0, 0), 0.1f);
                await Task.Delay(100);
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(1, 0, 0), 0.1f);
                await Task.Delay(100);
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, 1), 0.1f);
                await Task.Delay(100);
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, -2), 0.1f);
                await Task.Delay(100);
                await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, 1), 0.1f);

                await speechOut.Speak("so aim at him");
                //wiggling lef and right to show how to shoot                                                                                           LINO HELLIGE
                for (int i = 0; i <= 60; i+=10)
                {
                    float r = upperHandle.transform.eulerAngles.y + (float)i;
                    upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
                    await Task.Delay(100);
                }
                for (int i = 60; i >= -60; i -= 10)
                {
                    float r = upperHandle.transform.eulerAngles.y + (float)i;
                    upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
                    await Task.Delay(100);
                }
                for (int i = -60; i <= 0; i += 10)
                {
                    float r = upperHandle.transform.eulerAngles.y + (float)i;
                    upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
                    await Task.Delay(100);
                }
            break;
            //Level 2                                                                                                                                      OLIVER SCHULZ
            case 1:
                    await upperHandle.SwitchTo(player, 0.2f);
                    await speechOut.Speak("Oh no the enemy spotted you, move to another position...");
                    await MoveX();
                    await speechOut.Speak("...and kill the enemy...");
                    await lowerHandle.SwitchTo(enemy, 0.2f);
                    await speechOut.Speak("by aiming at him.");
                    await RotateX();
            break;
            //Level 3                                                                                                                                      OLIVER SCHULZ
            case 2:
                    await upperHandle.SwitchTo(player, 0.2f);
                    await lowerHandle.SwitchTo(player, 0.2f); //player shouldn't know where enemy is
                    await speechOut.Speak("Oh no the enemy escaped, move around...");
                    await MoveX();
                    await speechOut.Speak("...and watch out...");
                    await RotateX();
                    await speechOut.Speak("for the enemy to kill it.");
            break;

            default: break;
        }

        await speechOut.Speak("The gun will automaticly shoot for you");

    await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        //string response = await speechIn.Listen(commands);
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });
        //if (response == "yes")
        //{
        //    await RoomExploration();
        //}
    }

    async Task Move(float x, float y, float z, int n) //Move to position in n steps
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 v = new Vector3((float)x / n, y, (float)z / n);
            player.transform.position = player.transform.position + v;
            await Task.Delay(10);
        }
    }

    async Task MoveX() //Move in X
    {
        await Move(0, 0, 1, 10);
        await Move(0, 0, -1, 10);
        await Move(1, 0, 0, 10);
        await Move(-1, 0, 0, 10);
        await Move(0, 0, -1, 10);
        await Move(0, 0, 1, 10);
        await Move(-1, 0, -0, 10);
        await Move(1, 0, -0, 10);
    }

    async Task RotateX()//Rotate in X
    {
        for (int i = 0; i < 25; i++)
        {
            player.transform.Rotate(0, -1f, 0);
            await Task.Delay(10);
        }
        for (int i = 0; i < 50; i++)
        {
            player.transform.Rotate(0, 1f, 0);
            await Task.Delay(10);
        }
        for (int i = 0; i < 25; i++)
        {
            player.transform.Rotate(0, -1f, 0);
            await Task.Delay(10);
        }
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
        await upperHandle.SwitchTo(player, 0.3f);
        if (level == 0)                                                                                 //LINO HELLIGE
        {
            await upperHandle.SwitchTo(GameObject.Find("Player Spawn"), 0.3f);
        }
        await speechOut.Speak("Spawning enemy");
        enemy.transform.position = enemySpawn.position;
        enemy.transform.rotation = enemySpawn.rotation;
        await lowerHandle.SwitchTo(enemy, 0.3f);
        if (level >= enemyConfigs.Length)
            Debug.LogError($"Level {level} is over number of enemies {enemyConfigs.Length}");
        enemy.GetComponent<EnemyLogic>().config = enemyConfigs[level];

        if(level == 0)
        {
            upperHandle.FreeRotation();
        }
        else
        {
            upperHandle.Free();
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

        if (playerDefeated)
        {
            enemyScore++;
        }
        else
        {
            playerScore++;
        }
        uiManager.UpdateUI(playerScore, enemyScore);

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

            await uiManager.GameOver(gameScore, (int)totalTime, trophyScore);
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
