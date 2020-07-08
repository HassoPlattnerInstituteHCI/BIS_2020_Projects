using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using DualPantoFramework;

namespace Tetris {
public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool welcome = true;
    public bool introductoryLevel = true;
    public bool mainMenu = false;
    public GameObject player;
    public bool shouldFreeHandle;
    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    int playerScore = 0;

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

    async void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
            await Task.Delay(2000);
            RegisterColliders();
        if (mainMenu) {
            StartMainMenu();
        }
        if (welcome)
        {
            Welcome();
        } 
    }

    async void StartMainMenu() {
        await speechOut.Speak("Welcome to Tetris Panto Edition. Say modes to hear the list of available modes or start immediately by saying Tutorial.");
    }

    async void Welcome()    //welcome the player
    {

        if (introductoryLevel)
        {
            
            await IntroductoryLevel();
            await speechOut.Speak("Introduction finished, game starts.");

        } else{SpawnManager.spawnWavePls = true;} //If we are not in the welcome-Level, assume that we are in Endless and spawn a new wave
        //else-if to determine if in Puzzles mode + Puzzle-Spawn functio in SpawnManager.

        //await ResetGame();
    }

    async Task IntroductoryLevel()
    {
        await speechOut.Speak("Welcome to the Tutorial. We will now show you what you need to know to play the Tetris Panto Edition. Let's Start!");

        //Create a separate function here that progresses the Tutorial levels one by one. Start by deleting all remaining blocks in Scene (in Playfield.cs?),
        //so we can reuse it.

        //Spawn first intro level skyline
        SpawnManager.spawnIntroPls=true;
        
        await speechOut.Speak("The It-Handle will now trace the shape of the blocks on the bottom of the level, we will call this the 'skyline'.");

        //Playfield.traceSkyline();

        lowerHandle.Free();
        await speechOut.Speak("Now, try yourself to feel the blocks.");
        //Do we need to give the player control here? Remember to return to the block in the end.

        await Task.Delay(2000); //Changed time for debugging

        await speechOut.Speak("Now the Me-Handle will trace a block at the top of the level. Every block has its own type of sound, remember it!");
        // TODO: Me-handle trace

        await speechOut.Speak("Now, try to move the block down to clear a row of blocks in the skyline.");
        // TODO: Me-handle wiggle
    }

    void RegisterColliders()
    {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();

        foreach (PantoCollider collider in colliders)
        {
            if (collider.name.Contains("Border"))  //register border but not other colliders
            {
                collider.CreateObstacle();
                collider.Enable();
            }
        }
    }

    /// <summary>
    /// Starts a new round.
    /// </summary>
    /// <returns></returns>
    //async Task ResetGame()
    //{

    //}

    void onRecognized(string message)
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
        await speechOut.Speak("Thanks for playing Tetris Panto Edition. Say quit when you're done.");
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }
}
}