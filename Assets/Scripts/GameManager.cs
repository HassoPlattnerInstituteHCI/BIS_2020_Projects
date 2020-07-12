using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using DualPantoFramework;
using System.Collections;

namespace Tetris {
public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool welcome = true;
    public bool introductoryLevel = false;
    public bool mainMenu = false;
    public bool puzzles = false;
    public bool endless = false;
    public GameObject player;
    public bool shouldFreeHandle;

    public bool blockPlaced=false; //Let the Tutorial know when a block was placed
    public int clearCounter=0; //Counting the amount of rows cleared for Tutorial and Puzzles
    bool resetCurrentLevel = false; //If the player cannot finish the level, reset it

    UpperHandle upperHandle;
    public LowerHandle lowerHandle;

    GameObject upperPosition;
    GameObject lowerPosition;
    GameObject playerPosition;

    SpeechIn speechIn;
    SpeechOut speechOut;
    int playerScore = 0;

    GameObject SpawnerPos;

    Player PlayerIn;
    static int[] skylineHeights; //Each array space determines the height of the highest block in that column (in steps of 1, not .5)

    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();
        PlayerIn = GameObject.Find("Player").GetComponent<Player>();
    }

    async void Start()
    {
        SpawnerPos = GameObject.Find("SpawnerLeft");
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        upperPosition = GameObject.Find("MeHandlePrefab(Clone)").transform.GetChild(0).gameObject;
        lowerPosition = GameObject.Find("ItHandlePrefab(Clone)").transform.GetChild(1).gameObject;
        playerPosition = GameObject.Find("Player");

        await Task.Delay(2000);
        RegisterColliders();

        if (introductoryLevel)
        { 
            await IntroductoryLevel();
            await speechOut.Speak("Tutorial finished, you can now choose a new mode in the Main Menu.");
            MainMenu.PlayMainMenu();
        } else if(puzzles) {
            //TODO Make PuzzleManager Function
            SpawnManager.spawnWavePls = true; //Workaround for now
        } else if (endless) {
            SpawnManager.spawnWavePls = true;
        }
    }

    async Task IntroductoryLevel()
    {
    SpawnManager.introCounter=0;
    bool playingTutorial=true;
    while(playingTutorial) { //Implement some sort of way for the player to break this loop. Remember to set variable to false at end of last Level
        clearCounter=0;
        blockPlaced=false;
        await speechOut.Speak("Starting Tutorial Level Number "+(SpawnManager.introCounter+1));
        switch(SpawnManager.introCounter) {
            case 0 :
                await speechOut.Speak("Welcome to the Tutorial. We will now show you what you need to know to play the Tetris Panto Edition. Let's Start!");

                //Spawn first intro level skyline
                SpawnManager.spawnIntroPls=true;
        
                await speechOut.Speak("The It-Handle will now trace the shape of the blocks on the bottom of the level, we will call this the 'skyline'.");

                await traceSkyline();

                lowerHandle.Free();
                //await speechOut.Speak("Now, try yourself to feel the blocks.");
                //Do we need to give the player control here? Remember to return to the block in the end.

                await Task.Delay(1000); //Changed time for debugging

                await speechOut.Speak("Now the Me-Handle will trace a block at the top of the level. Every block has its own type of sound, remember it!");
                PlayerIn.onRecognized("left");

                await Task.Delay(1000);

                await speechOut.Speak("Now, try to move the block down to clear a row of blocks in the skyline. Say confirm to pick up the selected block. When you want to place the block, say place.");
                // TODO: Me-handle wiggle

                await WaitingForLevelFinish(SpawnManager.introCounter);
                if(resetCurrentLevel) {
                    resetCurrentLevel=false;
                    Playfield.cleanUpRows();
                    SpawnManager.introCounter--;
                    break;
                }

                await speechOut.Speak("You have successfully cleared your first rows! Congratulations!");

                Playfield.cleanUpRows();
                break;
            case 1:
                await speechOut.Speak("Now you have two blocks to choose from, but only one will fit in the skyline. Pay attention as the IT-Handle traces it.");
                await traceSkyline();

                lowerHandle.Free();

                await speechOut.Speak("The Me-Handle will now trace the left block. To change the active block, say 'right' or 'left'. Once you pick up a block the other one disappears, so choose wisely.");
                PlayerIn.onRecognized("left");
                await Task.Delay(2000);

                await WaitingForLevelFinish(SpawnManager.introCounter);
                if(resetCurrentLevel) {
                    resetCurrentLevel=false;
                    Playfield.cleanUpRows();
                    SpawnManager.introCounter--;
                    break;
                }

                await speechOut.Speak("Congratulations, you chose the right block and cleared the row!");
                Playfield.cleanUpRows();

                break;
            case 2:
                await speechOut.Speak("Welcome to the third level. This time you will learn how to rotate blocks to better fit in the skyline. Let's see how it looks like first.");
                await traceSkyline();

                lowerHandle.Free();

                await speechOut.Speak("The Me-Handle will always trace the left block first. After picking up a block, you can rotate it by saying 'rotate'. This will rotate the block 90° clockwise. Choose a block and rotate it to clear the skyline.");
                PlayerIn.onRecognized("left");
                await Task.Delay(2000);

                await WaitingForLevelFinish(SpawnManager.introCounter);
                if(resetCurrentLevel) {
                    resetCurrentLevel=false;
                    Playfield.cleanUpRows();
                    SpawnManager.introCounter--;
                    break;
                }

                await speechOut.Speak("Congratulations, you now know how to rotate blocks!");
                Playfield.cleanUpRows();
                break;
            case 3:
                await speechOut.Speak("You will not always be able to clear a row with the first block. Whenever you place a block, the It-Handle will trace the new skyline. Try it out yourself.");
                await traceSkyline();

                lowerHandle.Free();

                await speechOut.Speak("Choose any block and place it in the skyline. You will not be able to clear it.");
                PlayerIn.onRecognized("left");
                await Task.Delay(2000);

                await WaitingForLevelFinish(SpawnManager.introCounter);

                await speechOut.Speak("Great! In the final level you will learn what happens after placing a block.");
                Playfield.cleanUpRows();
                break;
            case 4:
                await speechOut.Speak("Welcome to the final level. You will now learn the last two mechanics: After placing a block, you always get two new ones. And after clearing a row, the blocks above it fall down to form the new skyline. The skyline in this level is the same as the one before.");
                await traceSkyline();

                lowerHandle.Free();

                await speechOut.Speak("Take your time and use as many blocks as you need to clear one row. Then you have completed the Tutorial!");
                PlayerIn.onRecognized("left");
                await Task.Delay(2000);

                await WaitingForLevelFinish(SpawnManager.introCounter);
                await Task.Delay(7000);
                await speechOut.Speak("Congratulations, you have completed the Tutorial!");
                Playfield.cleanUpRows();
                playingTutorial=false;
                introductoryLevel=false;
                break;
        }
        if(SpawnManager.introCounter!=4) {
            SpawnManager.introCounter++;
            SpawnManager.spawnIntroPls = true;
        }
    }
    }

    public async Task WaitingForLevelFinish(int levelNum) {
        switch(levelNum) {
            case 0:
                while(clearCounter!=2){
                    if(blockPlaced) {
                        await speechOut.Speak("It seems you have misplaced the block and cannot finish the level. Resetting level...");
                        resetCurrentLevel=true;
                        return;
                    }
                    await Task.Delay(100);
                }
                break;
            case 1:
            case 2:
                while(clearCounter!=1) { 
                    if(blockPlaced) {
                        await speechOut.Speak("It seems you have misplaced the block and cannot finish the level. Resetting level...");
                        resetCurrentLevel=true;
                        return;
                    }
                    await Task.Delay(100);
                }
                break;
            case 3: while(!blockPlaced) {await Task.Delay(100);}
                break;
            case 4: while(clearCounter!=1) {await Task.Delay(100);}
                break;
        }
    }

    public async Task traceBlock(int blockCode, bool isLeft) {
        
        switch(blockCode) {
            case 0: 
            await speechOut.Speak("Block Eye");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.25f,0f,-1f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,2f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-2f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-0.5f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 1:
            await speechOut.Speak("Block L");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.5f, 0f, -0.75f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,1.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-1f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 2:
            await speechOut.Speak("Block Reverse L");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.5f, 0f, -0.75f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-1f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 3: 
            await speechOut.Speak("Block O");
                await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.5f, 0f, -0.5f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(1f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-1f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 4:
            await speechOut.Speak("Block S");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.5f, 0f, -0.5f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(1f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-1f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 5:
            await speechOut.Speak("Block Z");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-0.5f, 0f, -0.5f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-1f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-0.5f,0f,0f), 0.1f, shouldFreeHandle);
                break;
            case 6:
            await speechOut.Speak("Block T");
            await upperHandle.MoveToPosition(playerPosition.transform.position + new Vector3(-1.25f, 0f, -0.25f), 0.1f, shouldFreeHandle); //To correct global position
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(0f,0f,-0.5f), 0.1f, shouldFreeHandle);
                await upperHandle.MoveToPosition(upperPosition.transform.position + new Vector3(-1.5f,0f,0f), 0.1f, shouldFreeHandle);
                break;
        }
        await upperHandle.MoveToPosition(playerPosition.transform.position, 0.1f, shouldFreeHandle);
    }

    public async Task traceSkyline() {
        await speechOut.Speak("Tracing the skyline.");
        //In the first part, we get the heights of the highest block in each column. For this, we need to go through every row, starting with the highest one
        //If there is a block in there with a column-tag that is not yet initialized in the array, we take it as our highest block.
        skylineHeights = new int[11]; //By default, max height is 0=ground level
        GameObject currentRow;
        int column;
        //Every step in the array signals one step of .5 upwards when tracing. This means a block in row 1 has a value of 2 in the array!
        for(int row=Playfield.h-1; row>=0; row--) { //for every row...
            currentRow = Playfield.allRowsParent.transform.GetChild(row).gameObject;
            foreach(Transform child in currentRow.transform) { //...check all blocks in that row for their tag and check if that column is already !=0 in the array
            column = int.Parse(child.tag);
                if(skylineHeights[column+1]==0) { //Do the check only if the space has not been occupied beforehand
                    skylineHeights[column+1]=row+1;
                }
                //Debug.Log("SkylineRow :"+row+" Height :"+skylineHeights[column+1]+" column :"+column);
            }
        }
        //We have now filled the Array with the heights in each column. Time to make the Panto move
        await lowerHandle.MoveToPosition(new Vector3(-2.25f,0f,-14.25f), 0.1f, shouldFreeHandle); //Moves handle to lower left corner of the level
        for(int col=1; col<11; col++) { //Starting at 1 since skylineHeights[0] is our default value for the first subtraction below
        Debug.Log(lowerHandle.transform.position);
            await lowerHandle.MoveToPosition(lowerPosition.transform.position + new Vector3(0f,0f,0.5f*(skylineHeights[col]-skylineHeights[col-1])), 0.1f, shouldFreeHandle);
            await lowerHandle.MoveToPosition(lowerPosition.transform.position + new Vector3(0.5f,0f,0f), 0.1f, shouldFreeHandle);
        }
        await lowerHandle.MoveToPosition(new Vector3(2.75f,0f,-14.25f), 0.1f, shouldFreeHandle);
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