using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Security;
using UnityEditorInternal;
using UnityEditor;
using System.Net.Sockets;
using System.Threading;
using DualPantoFramework;
using System.Threading.Tasks;

namespace Tetris {
public class Player : MonoBehaviour
{
    private PantoHandle meHandle;
    GameObject meHandlePrefab;
    GameObject activeBlock;
    public static int activeBlockID;
    public static Vector3 leftBlockRootPos;
    public static Vector3 rightBlockRootPos;
    bool playercontrol = false;
    bool chooseMode = true;
    bool leftBlockActive = true;
    bool placement = false;
    public bool shouldFreeHandle;
    public int rotateAmount; //To track how the block looks in a give moment when trying to rotate
    public float movementspeed = 0.2f;
    public GameObject SpawnerLeft;
    SpeechIn speechIn;
    SpeechOut speechOut;
    GameManager Manager;
    Playfield Field;
    
    
/*
    public int startBPM = 60;
    public int endBPM = 220;
    float bpmCoefficient;
    public float bps = 1;
    float nextHeartbeat;
    */
        // Start is called before the first frame update

        void Awake()
    {
        Manager = GameObject.Find("Panto").GetComponent<GameManager>();
        Field = GameObject.Find("BackgroundWhite").GetComponent<Playfield>();
        
    }

    async void Start()
    {
        
        speechOut = new SpeechOut();
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        await Task.Delay(1000);
        //await meHandle.MoveToPosition(transform.position, 0.2f);
        //await speechOut.Speak("Welcome to Tetris Panto Edition.");
        speechIn = new SpeechIn(onRecognized, new string[] { "left", "right", "confirm","rotate", "place", "abort" });
        speechIn.StartListening(new string[] {"left", "right", "confirm","rotate", "place", "abort" });
    }

    // Update is called once per frame
    void Update()
    {
            // Simply connects the player to the upper handles position

        if (!playercontrol) {
                if (Input.GetKeyDown(KeyCode.L)) {
                    onRecognized("left");
                }
                if (Input.GetKeyDown(KeyCode.R)) {
                    onRecognized("right");
                }
                if (Input.GetKeyDown(KeyCode.C)) {
                    onRecognized("confirm");
                }
                if (Input.GetKeyDown(KeyCode.X)) {
                    onRecognized("abort");
                }
            }
        if (playercontrol) {
            transform.position = meHandle.HandlePosition(transform.position);
            activeBlock.transform.position = transform.position;
            Field.alignLive(activeBlock, activeBlockID);
            // Rotate !!Need way of doing this with the Me-Handle rotation!!
            if (Input.GetKeyDown(KeyCode.Space)) {
                onRecognized("rotate");
            }

            else if (Input.GetKeyDown(KeyCode.P)) {
                onRecognized("place");
            }
            else if (Input.GetKeyDown(KeyCode.C)) {
                onRecognized("confirm");
            }
        }

    }
    
    public async void onRecognized(string message)
    {
        //REENABLE SPEECHOUT BEFORE BUILDING
        //checking voice input
        Debug.Log("[" + this.GetType() + "]:" + message);
        if (message == "left" && !playercontrol && !placement)      //select left block
        {
            //await meHandle.MoveToPosition(leftBlockRootPos, 0.3f, shouldFreeHandle);
            transform.position = leftBlockRootPos;
            //TODO: Sound
            //await speechOut.Speak("Now tracing the left block");
            await Manager.traceBlock(SpawnManager.leftBlock, true);
            leftBlockActive = true;
            activeBlockID = SpawnManager.leftBlock;
            Manager.blockPlaced=false;
        }
        if (message == "right" && !playercontrol && !placement)     //select right block
        {
            //await meHandle.MoveToPosition(rightBlockRootPos, 0.3f, shouldFreeHandle);
            transform.position = rightBlockRootPos;
            //await speechOut.Speak("Now tracing the right block");
            //TODO: Sound
            await Manager.traceBlock(SpawnManager.rightBlock, false);
            leftBlockActive = false;
            activeBlockID = SpawnManager.rightBlock;
            Manager.blockPlaced=false;
        }
        if(message == "confirm" && !playercontrol && !placement)    //confirm block selection
        {
            meHandle.Free();
            await speechOut.Speak("Block picked up.");
            if(leftBlockActive) {
                Destroy(SpawnManager.blockRight);
                SpawnManager.blockLeft.transform.SetParent(transform);
                activeBlock = SpawnManager.blockLeft;
                
            } else {
                Destroy(SpawnManager.blockLeft); 
                SpawnManager.blockRight.transform.SetParent(transform);
                activeBlock = SpawnManager.blockRight; 
                }
            
            playercontrol = true;
            chooseMode = false; //remove this
            rotateAmount=0;
            activeBlock.transform.position = transform.position;
        }
        if(message == "rotate" && playercontrol) {
            //await speechOut.Speak("Rotating");
            rotateAmount = Field.rotateBlock(activeBlock, activeBlockID, rotateAmount);
        }
        if(message == "place" && playercontrol)     //placing the block on the grid                     
        {
            if(Playfield.isValidPlacement(activeBlock)) {
                Field.audioSource.PlayOneShot(Field.BlockPlace, Field.volume);
                placement = true;
                playercontrol = false;
                await meHandle.MoveToPosition(activeBlock.transform.GetChild(0).transform.position, 0.3f, shouldFreeHandle);
                //await speechOut.Speak("Block can be placed here. Say confirm or abort to continue.");
            } else {await speechOut.Speak("You cannot place the block here.");}
        }
        if(message == "confirm" && placement)       //confirming placement location
        {
            await speechOut.Speak("Block placed."); //TODO Sound
            activeBlock.transform.parent = null; //detach Block from Player
            placement = false;
            Playfield.confirmBlock(activeBlock);
            await Field.deleteFullRows();
            Manager.blockPlaced=true;
            if(!Manager.introductoryLevel || (SpawnManager.introCounter==4 && Manager.clearCounter==0) ) {
                SpawnManager.spawnWavePls = true;
                transform.position = SpawnerLeft.transform.position;
                await Task.Delay(500);
                await meHandle.MoveToPosition(leftBlockRootPos, 0.3f, shouldFreeHandle);
                //Initializes next wave on the Me-Handle immediately
                onRecognized("left");
                await Task.Delay(1000);
            }
        }
        if(message == "abort" && placement)     //abort block placement
        {
            await speechOut.Speak("Placement aborted.");
            placement = false;
            playercontrol = true;
            meHandle.Free();
        }
    }
}
}