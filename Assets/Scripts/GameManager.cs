using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool introduceLevel = true;
    public Transform playerSpawn;
    static public int level = 0;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    int hitCount = 0;
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
        await lowerHandle.SwitchTo(GameObject.Find("Ball"), 0.2f);
        await speechOut.Speak("Welcome to PantoGolf");
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
        await speechOut.Speak("There are no obstacles.");
        Level level = GetComponent<Level>();
        await level.PlayIntroduction();

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
        level = 0;
        LoadScene(level);
    }

    public void LevelComplete()
    {
        Debug.Log("You completed the level.");
        level++;
        LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));
    }

    public void RestartLevel()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        await speechOut.Speak("Thanks for playing PantoGolf.");
        Application.Quit();
    }

    private void LoadScene(int index)
    {
        Debug.Log("Try to load Scene with index " + index);
        SceneManager.LoadScene(index);
    }
}
