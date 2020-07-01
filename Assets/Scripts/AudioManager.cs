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
    private Action<string> onCreate;
    Action<string> onDelete;
    Action<string> onShow;
    private Action onList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Audiomanager initialized");
        speechOut = new SpeechOut();
    }

    async public Task Say(string text)
    {
        await speechOut.Speak(text);
    }
    public void SetCallbacks(Action<string> selectCallback,
                             Action<string> createCallback,
                             Action<string> deleteCallback,
                             Action listCallback,
                             Action<string> showCallback)
    {
        onSelect = selectCallback;
        onCreate = createCallback;
        onDelete = deleteCallback;
        onList = listCallback;
        onShow = showCallback;
    }
    
    public void UpdateCommands(GameObject[] elements)
    {
        if (speechIn != null){
            speechIn.StopListening();
            speechIn = null;
        }
        
        List<string> newCommands = new List<string> {};
        foreach (GameObject element in elements)
        {
            newCommands.Add($"Select {element.name}");
            newCommands.Add($"Select {element.name}");
        }

        // exemplary cause bug
        newCommands.Add("Create Tree");
        newCommands.Add("Create Car");
        newCommands.Add("Delete Otter");
        newCommands.Add("List Elements");
        newCommands.Add("Show Sun");

        speechIn = new SpeechIn(OnRecognize, newCommands.ToArray());

        speechIn.StartListening();
    }

    private void OnRecognize(string command){
        Debug.Log(command);
        if (command == "List Elements")
        {
            onList();
        }
        if (command.StartsWith("Create"))
        {
            onCreate(command.Substring("Create ".Length));
        }
        if (command.StartsWith("Select"))
        {
            onSelect(command.Substring("Select ".Length));
        }
        if (command.StartsWith("Delete"))
        {
            onDelete(command.Substring("Delete ".Length));
        }
        if (command.StartsWith("Show"))
        {
            onShow(command.Substring("Show ".Length));
        }
    } 

    public void OnApplicationQuit()
    {
        if (speechIn != null)
        {
            speechIn.StopListening();
        }
      
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
