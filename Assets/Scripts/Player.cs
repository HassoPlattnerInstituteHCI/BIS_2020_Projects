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
    public static Vector3 leftBlockRotaterPos;
    public static Vector3 rightBlockRotaterPos;
    bool playercontrol = false;
    bool chooseMode = true;
    bool leftBlockActive = true;
    bool placement = false;
    public bool shouldFreeHandle;
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
        await meHandle.SwitchTo(SpawnerLeft, 0.4f);
        //await speechOut.Speak("Welcome to Tetris Panto Edition.");
        speechIn = new SpeechIn(onRecognized, new string[] { "left", "right", "confirm", "place", "abort" });
        speechIn.StartListening(new string[] {"left", "right", "confirm", "place", "abort" });
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
            
            // Rotate !!Need way of doing this with the Me-Handle rotation!!
            if (Input.GetKeyDown(KeyCode.Space)) {
                activeBlock.transform.RotateAround(activeBlock.transform.GetChild(0).position, new Vector3(0,1,0), -90);
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
        //checking voice input
        Debug.Log("[" + this.GetType() + "]:" + message);
        if (message == "left" && !playercontrol && !placement)      //select left block
        {
            await meHandle.MoveToPosition(leftBlockRotaterPos, 0.3f, shouldFreeHandle);
            transform.position = leftBlockRotaterPos;
            //SpawnManager.blockLeft.transform.SetParent(transform);
            //TODO: Sound
            await speechOut.Speak("Now tracing the left block");
            await Manager.traceBlock(SpawnManager.leftBlock, true);
            leftBlockActive = true;
        }
        if (message == "right" && !playercontrol && !placement)     //select right block
        {
            await meHandle.MoveToPosition(rightBlockRotaterPos, 0.3f, shouldFreeHandle);
            transform.position = rightBlockRotaterPos;
            //SpawnManager.blockRight.transform.SetParent(transform);
            await speechOut.Speak("Now tracing the right block");
            //TODO: Sound
            await Manager.traceBlock(SpawnManager.rightBlock, false);
            leftBlockActive = false;
        }
        if(message == "confirm" && !playercontrol && !placement)    //confirm block selection
        {
            if(leftBlockActive) {
                Destroy(SpawnManager.blockRight);
                SpawnManager.blockLeft.transform.SetParent(transform);
                activeBlock = SpawnManager.blockLeft;
                
            } else {
                Destroy(SpawnManager.blockLeft); 
                SpawnManager.blockRight.transform.SetParent(transform);
                activeBlock = SpawnManager.blockRight; 
                }
            meHandle.Free();
            playercontrol = true;
            chooseMode = false;
            
            activeBlock.transform.position = transform.position;
        }
        if(message == "place" && playercontrol)     //placing the block on the grid                     
        {
            if(Playfield.isValidPlacement(activeBlock)) {
                placement = true;
                playercontrol = false;
                Playfield.roundAndPlaceBlock(activeBlock);
                await meHandle.MoveToPosition(activeBlock.transform.GetChild(0).transform.position, 0.3f, shouldFreeHandle);
            } else {await speechOut.Speak("You cannot place the block here.");}
        }
        if(message == "confirm" && placement)       //confirming placement location
        {
            activeBlock.transform.parent = null; //detach Block from Player
            placement = false;
            Playfield.confirmBlock(activeBlock);
            Field.deleteFullRows();
            Manager.blockPlaced = true;
            if(!Manager.introductoryLevel) {
                SpawnManager.spawnWavePls = true;
                transform.position = SpawnerLeft.transform.position;
                await meHandle.MoveToPosition(leftBlockRotaterPos, 0.3f, shouldFreeHandle);
                //Initializes next wave on the Me-Handle immediately
                onRecognized("left");
            }
        }
        if(message == "abort" && placement)     //abort block placement
        {
            placement = false;
            playercontrol = true;
            meHandle.Free();
        }
    }
}
}