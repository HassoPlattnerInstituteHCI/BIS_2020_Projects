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
    public GameObject[] enemies;
    public Transform playerSpawn;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;

    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
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

        Introduction();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to Stealth for  Dual Panto.");
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
        string response = await speechIn.Listen(commands);
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

        //if (response == "yes")
        //{
        //    await RoomExploration();
        //}
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

        await speechOut.Speak("Spawning enemy");
        // await lowerHandle.SwitchTo(enemy, 0.3f);

        upperHandle.Free();

        player.SetActive(true);
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
    /// Ends the game.
    /// </summary>
    /// <returns></returns>
    async Task GameOver()
    {
        await speechOut.Speak("Congratulations.");

        await speechOut.Speak("Thanks for playing stealth. Say quit when you're done.");
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }
}
