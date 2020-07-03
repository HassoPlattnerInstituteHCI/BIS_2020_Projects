﻿using System.Threading.Tasks; //note that you need to include this if you want to use Task.Delay.
using UnityEngine;
using SpeechIO;

public class MyDialogScript : MonoBehaviour
{
    SpeechIn speechIn;
    SpeechOut speechOut;
    void Start()
    {
        speechIn = new SpeechIn(onRecognized);
        speechOut = new SpeechOut();
        Dialog(); //Separately run asynchronous Dialog system.
    }

    private async void Dialog()
    {
        speechIn.StartListening(); //startup native speech
        await Task.Delay(1000);
        await speechOut.Speak("Welcome to the DualPanto Corona Diagnosis App");
        await speechOut.Speak("Time to greet me");
        //Debug.Log("Hello!"); // system says hello
        await speechIn.Listen(new string[] { "hello", "hi", "hey" }); //wait for greet 
        //await Task.Delay(2000); // wait 2s 
        await speechOut.Speak("How are you?!");
        string health = await speechIn.Listen(new string[] { "I'm fine", "so-so", "I'm sick" }); //wait for response
        switch (health)
        { // switch response
            case "I'm fine":
                Debug.Log("super!");
                break;
            default:
                Debug.Log("alright, take care.");
                await speechOut.Speak("crazylaugh");
                await speechOut.Speak("you really thought I would diagnose you?!");
                break;
        }
        speechIn.StopListening(); //terminates voice recognition
    }

    void onRecognized(string message)
    {
        Debug.Log("[" + this.GetType() + "]: " + message);
    }

    public void OnApplicationQuit()
    {
        speechIn.StopListening(); // [mac] do not delete this line!
        speechOut.Stop();
    }
}
