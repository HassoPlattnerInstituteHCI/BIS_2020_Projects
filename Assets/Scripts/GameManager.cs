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
    public int currentLevel;

    private TelephoneSoundEffect telephoneSounds;
    private PlayerSoundEffect playerSounds;

    GameObject phoneBox;
    GameObject bat;

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
        
        phoneBox = GameObject.Find("TelephoneBox1");

        bat = GameObject.Find("Bat");
        bat.SetActive(false);

        telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();

        //uiManager.UpdateUI(playerScore, enemyScore);

        Introduction();
    }

    async void Introduction() //Speech: Introduce Me-Handle = Move. - Go to telephone
    {
        await speechOut.Speak("Use the upper handle to move your character. Spawning Player");        
        await ResetGame();

        //await Task.Delay(1000);
        RegisterColliders();

        if (introduceLevel)
        {
            await StartLevel1();
        }

        
    }

    async Task StartLevel1()
    {
        currentLevel = 1;
        await speechOut.Speak("Pick up the phone");      

        bat.SetActive(true);

        lowerHandle.SwitchTo(bat, 0.1f);

        transform.rotation = Quaternion.Euler(0, upperHandle.GetRotation(), 0);
       
        
        //phone starts ringing
        telephoneSounds.startPhoneRing();


        //TODO: If player is close to Phone BOX: -> Speechout Johnny Zoo
        
        //Level level = GetComponent<Level>();
        //await level.PlayIntroduction();

        //await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        //string response = await speechIn.Listen(commands);
        //await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

        //if (response == "yes")
        //{
        //    await RoomExploration();
        //}

    }

    public async Task StartLevel2(){
        currentLevel = 2;
        playerSpawn.position = phoneBox.transform.position;
        phoneBox = GameObject.Find("TelephoneBox2");
        telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();
        telephoneSounds.startPhoneRing();

    }

    public async Task StartLevel3(){
        currentLevel = 3;
        playerSpawn.position = phoneBox.transform.position;
        player = GameObject.Find("Player");
        playerSounds = player.GetComponent<PlayerSoundEffect>();
        Debug.Log("start level 3");
        playerSounds.startSirens();

    }

    

    void RegisterColliders() {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();

    //TODO: ADD all obstacles and walls to the game

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
        
        player.transform.position = playerSpawn.position;
        await upperHandle.MoveToPosition(player.transform.position, 0.2f, true);

        //upperHandle.Free();

        player.SetActive(true);
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<AudioListener>().enabled = false;
        levelStartTime = Time.time;
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

        gameScore += CalculateGameScore(player, enemy); //TODO Level 2

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
