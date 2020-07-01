using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System;

public class AudioManager : MonoBehaviour
{

    private SpeechIn speechIn;
    private SpeechOut speechOut; 
    private Action<string> onSelect; 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Audiomanager initialized");
        speechOut = new SpeechOut();
    }

    public void SetCallbacks(Action<string> selectCallback)
    {
        onSelect = selectCallback;
    }
    
    public void UpdateCommands(GameObject[] elements)
    {
        List<string> newCommands = new List<string> {};
        foreach (GameObject element in elements)
        {
                newCommands.Add($"Select {element.name}");
        }
        speechIn = new SpeechIn(OnRecognize, newCommands.ToArray());
        speechIn.StartListening();
    }

    private void OnRecognize(string command){
        if (command.StartsWith("Select"))
        {
            onSelect(command.Substring("Select ".Length));
        }
    } 

    public void OnApplicationQuit()
    {
        speechIn.StopListening();    
    }

    async public Task IntroduceFirstElement()
    {
        await speechOut.Speak("Here is a graphic of a sun.");
    }  
    // Update is called once per frame
    void Update()
    {
        
    }
}
