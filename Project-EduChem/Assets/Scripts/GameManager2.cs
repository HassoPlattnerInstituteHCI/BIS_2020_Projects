using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


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
        await speechOut.Speak("Level2 started");
            await IntroduceLevel();
        //}

        //await FirstLevel();
        //await FeelForYourself();

        await SecondLevel();

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
