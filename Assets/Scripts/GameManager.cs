using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool welcome = true;
    public static bool introductoryLevel = true;
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

    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        RegisterColliders();
        if (welcome)
        {
            Welcome();
        }
    }

    async void Welcome()    //welcome the player
    {
        
        await speechOut.Speak("Welcome to Tetris Panto Edition");
          
        await Task.Delay(1000);

        if (introductoryLevel)
        {
            await IntroductoryLevel();
        }
        
        await speechOut.Speak("Introduction finished, game starts.");
        //SceneManager.LoadScene("Endless");  //Endless level

        //await ResetGame();
    }

    async Task IntroductoryLevel()
    {
        //Register Blocks in Grid
        /*
        Playfield.confirmBlock(GameObject.Find("BlockL2"));
        Playfield.confirmBlock(GameObject.Find("BlockZ"));
        Playfield.confirmBlock(GameObject.Find("BlockL"));
        Playfield.confirmBlock(GameObject.Find("BlockZ2"));
        Playfield.confirmBlock(GameObject.Find("BlockI"));
        Playfield.confirmBlock(GameObject.Find("BlockL22"));
        */
        //Spawn first intro level skyline
        SpawnManager.spawnIntroPls=true;
        SpawnManager.introCounter=0;

        await speechOut.Speak("The It-Handle will now trace the shape of the blocks on the bottom of the level, we will call this the 'skyline'.");
        //yes there propably is a better way to do this
        //Idea: Using the grid, for each column find the highest positioned block. Move there, then .5 to the right, find the next one in relative position to current
        //->this will however ignore "holes" in the skyline
        await lowerHandle.MoveToPosition(new Vector3(0f,0f,2f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(0.5f, 0f, 2f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(0.5f, 0f, 1f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(1f, 0f, 1f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(1f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(2f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(2f, 0f, 0f), 0.1f, shouldFreeHandle); //Weird random "New Game Object" is created somewhere in this area?!?
        await lowerHandle.MoveToPosition(new Vector3(2.5f, 0f, 0f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(2.5f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(3f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(3f, 0f, 1f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(4.5f, 0f, 1f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(4.5f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        await lowerHandle.MoveToPosition(new Vector3(5f, 0f, 0.5f), 0.1f, shouldFreeHandle);
        lowerHandle.Free();
        await speechOut.Speak("Now, try yourself to feel the blocks.");
        //Do we need to give the player control here? Remember to return to the block in the end.

        await Task.Delay(20000);

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
