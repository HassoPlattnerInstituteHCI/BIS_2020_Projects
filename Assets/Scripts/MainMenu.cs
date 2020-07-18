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

    public static int highscoreOverall = 0;
    public static bool playIntro = false;

    SpeechIn speechIn;
    SpeechOut speechOut;

    async void Start() {
        speechOut = new SpeechOut();
        
        speechIn = new SpeechIn(onRecognized, new string[] {"tetris", "modes", "tutorial", "highscore", "back", "quit", "commands" });
        await speechOut.Speak("Welcome to Tetris Panto Edition. Say Tutorial or Highscore to start playing. Say 'Tetris' to get an explanation of the game.");
        speechIn.StartListening(new string[] {"tetris", "modes", "tutorial", "highscore", "back", "quit", "commands"});
    }

    public static void PlayMainMenu() {
        SceneManager.LoadScene(0);
    }

    public void PlayTutorial() {
        playIntro = true;
        SceneManager.LoadScene(1); //loads scene 1 aka Introduction (index in build settings needs to be 1)
    }

    public void PlayEndless() {
        playIntro=false;
        SceneManager.LoadScene(1); //loads endless scene (must be index 2)
    }
    /*
    public void PlayPuzzles() {
        SceneManager.LoadScene(3); //loads Puzzles-Scene. Need to implement Level-select if time
    }
    */
    public void QuitGame () {
        Debug.Log ("QUIT!");
        Application.Quit();
    }

async void onRecognized(string message)
    {
        var pointer = new PointerEventData(EventSystem.current); // pointer event for Execute
        Debug.Log("[" + this.GetType() + "]:" + message);
        if(message == "tetris") {
            await speechOut.Speak("Tetris Panto Edition is a Puzzle game where you get blocks each turn. A block is a shape, such as an L, and always consists of 4 blocks. Your goal is to move the block downwards in a rectangular level.");
            await speechOut.Speak("As soon as the block is on the ground or on top of another block, you can place it, making it a part of the level. After placing a block, the game will delete any full rows in the level, where each small space in a row is occupied by a block you placed.");
            await speechOut.Speak("Whenever you clear a line, your score increases. The more lines you clear at once, the higher the score added is. One line gets you 40 points, two lines 100, three lines 300 and four lines 1200 points!");
            await speechOut.Speak("In Highscore mode the game ends after 20 waves. In the Tutorial you will learn the basics of how to play Tetris Panto Edition.");
        }
        else if (message == "modes")
        {
            await speechOut.Speak("You can choose one of the following modes: Tutorial, Highscore.");
        }
        else if (message == "tutorial")
        {
            await speechOut.Speak("Starting Tutorial.");
            ExecuteEvents.Execute(TutorialButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }/*
        else if (message == "puzzles")
        {
            await speechOut.Speak("Starting first Puzzle.");
            ExecuteEvents.Execute(PuzzlesButton.gameObject, pointer, ExecuteEvents.submitHandler);
        }*/
        else if (message == "highscore")
        {
            await speechOut.Speak("Starting Highscore mode.");
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
            await speechOut.Speak("The following commands are available: Modes, Tutorial, Highscore, Quit.");
        }
        else {
            await speechOut.Speak("You cannot use this command here. Say commands to here the list of available commands.");
        }
    }

}
}