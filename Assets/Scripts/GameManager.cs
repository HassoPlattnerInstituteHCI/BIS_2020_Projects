﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System.Linq;

public class GameManager : MonoBehaviour
{
    
    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    private SpeechIn speechIn;
    private SpeechOut speechOut;
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        //{ "yes", KeyCode.Y },
        { "no", KeyCode.N },
        //{ "done", KeyCode.D },
        { "add", KeyCode.A }
    };
    
    private int levelNumber = 1;
    public bool introduceLevel = true;

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();

        /*if (level < 0 || level >= enemyConfigs.Length)
        {
            Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");
            level = 0;
        }*/
        
    }
    

    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        Debug.Log("Before Introduction");
        speechIn.StartListening();
        Introduction();
        
    }

    async void onRecognized(string message)
    {
        //WIP
        switch (message)
        {
            case "add":
                string name;
                //name = await speechIn.Listen();
                break;
            case "repeat":
                await speechOut.Repeat();
                break;
            case "quit":
                await speechOut.Speak("Thanks for using our application. Closing down now...");
                OnApplicationQuit();
                Application.Quit();
                break;
            /*case "options":
                string commandlist = "";
                foreach (string command in commands.Keys)
                {
                    commandlist += command + ", ";
                }
                await speechOut.Speak("currently available commands: " + commandlist);
                break;*/
        }
    }
    
    private void OnApplicationQuit()
    {
        speechOut.Stop();
        speechIn.StopListening();
    }

    async void Introduction()
    {
        await speechOut.Speak("Welcome to Panto Drawing");
        // TODO: 1. Introduce obstacles in level 2 (aka 1)
        await Task.Delay(1000);
        RegisterColliders();

        /*if (introduceLevel)
        {
            await IntroduceLevel(levelNumber);
        }*/

        await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        //string response = await speechIn.Listen(commands);
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });

        await speechOut.Speak("Introduction finished, game starts.");

        //await ResetGame();
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
}
