using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpeechIO;
using DualPantoFramework;
using UnityEngine.EventSystems;

namespace Tetris {
public class MainMenu : MonoBehaviour
{

    public Button QuitButton, OptionsButton, TutorialButton, EndlessButton, PuzzlesButton, BackButton;

    SpeechIn speechIn;
    SpeechOut speechOut;

    void Start() {
        speechOut = new SpeechOut();
        speechIn = new SpeechIn(onRecognized, new string[] { "modes", "tutorial", "endless", "puzzles", "back", "quit", "commands" });
        speechIn.StartListening(new string[] {"modes", "tutorial", "endless", "puzzles", "back", "quit", "commands"});
    }

    public void PlayTutorial() {
        SceneManager.LoadScene(1); //loads scene 1 aka Introduction (index in build settings needs to be 1)
    }

    public void PlayEndless() {
        SceneManager.LoadScene(2); //loads endless scene (must be index 2)
    }

    public void PlayPuzzles() {
        SceneManager.LoadScene(3); //loads (for now unexisting) Puzzles-Scene. Need to implement Level-select if time
    }

    public void QuitGame () {
        Debug.Log ("QUIT!");
        Application.Quit();
    }

async void onRecognized(string message)
    {
        var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
        Debug.Log("[" + this.GetType() + "]:" + message);
        if (message == "modes")
        {
            await speechOut.Speak("You can choose one of the following modes: Tutorial, Puzzles, Endless.");
        }
        else if (message == "tutorial")
        {
            await speechOut.Speak("Starting Tutorial.");
            ExecuteEvents.Execute(TutorialButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        else if (message == "puzzles")
        {
            await speechOut.Speak("Starting first Puzzle.");
            ExecuteEvents.Execute(PuzzlesButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        else if (message == "endless")
        {
            await speechOut.Speak("Starting Endless mode.");
            ExecuteEvents.Execute(EndlessButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        else if (message == "back")
        {
            await speechOut.Speak("Going back to Main Menu.");
            ExecuteEvents.Execute(BackButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        else if (message == "quit")
        {
            await speechOut.Speak("Quitting the game. Thanks for playing.");
            ExecuteEvents.Execute(QuitButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }
        else if (message == "commands")
        {
            await speechOut.Speak("The following commands are available: Modes, Tutorial, Puzzles, Endless, Quit, Back.");
        }
        else {
            await speechOut.Speak("You cannot use this command here. Say commands to here the list of available commands.");
        }
    }

}
}