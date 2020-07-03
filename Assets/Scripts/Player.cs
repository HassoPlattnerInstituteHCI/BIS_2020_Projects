using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Security;
using UnityEditorInternal;
using UnityEditor;
using System.Net.Sockets;
using System.Threading;

public class Player : MonoBehaviour
{
    private PantoHandle meHandle;
    GameObject meHandlePrefab;
    GameObject activeBlock;
    bool playercontrol = false;
    bool chooseMode = true;
    bool leftBlockActive = false;
    bool movedOnce = false;
    bool placement = false;
    public bool shouldFreeHandle;
    public float movementspeed = 0.2f;
    public GameObject SpawnerLeft;
    SpeechIn speechIn;
    SpeechOut speechOut;
    // Start is called before the first frame update

    void Awake()
    {
        
        
    }

    async void Start()
    {
        
        speechOut = new SpeechOut();
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        await meHandle.SwitchTo(gameObject, 0.4f);
        //movementStarted = true;
        await speechOut.Speak("Welcome to Tetris Panto Edition");
        //Is this the right place for it? Will need it in Update to work also for the next waves of blocks
        speechIn = new SpeechIn(onRecognized, new string[] { "left", "right", "confirm", "place", "abort" });
        speechIn.StartListening(new string[] {"left", "right", "confirm", "place", "abort" });
        //meHandlePrefab = GameObject.Find("MeHandlePrefab(Clone)");
    }

    // Update is called once per frame
    async void Update()
    {

            if(!playercontrol) {
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
            //From here on movement via Keyboard arrows for now. Need to couple with Me-Handle movements.
            // Move Left
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                // Modify position
                //meHandlePrefab.transform.position += new Vector3((float)-0.5, 0, 0);
                transform.position += new Vector3((float)-0.5, 0, 0);
            }

            // Move Right
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                // Modify position
                //meHandlePrefab.transform.position += new Vector3((float)0.5, 0, 0);
                transform.position += new Vector3((float)0.5, 0, 0);
            }

            // Move Up
            else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                // Modify position
                //meHandlePrefab.transform.position += new Vector3(0, 0, (float)0.5);
                transform.position += new Vector3(0, 0, (float)0.5);
            }

            // Move Downwards
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                // Modify position
                //meHandlePrefab.transform.position += new Vector3(0, 0, (float)-0.5);
                transform.position += new Vector3(0, 0, (float)-0.5);
            }
            // Rotate
            else if (Input.GetKeyDown(KeyCode.Space)) {
                //meHandlePrefab.transform.Rotate(0, -90, 0);
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
    
    async void onRecognized(string message)
    {
        Debug.Log("[" + this.GetType() + "]:" + message);
        if (message == "left" && !playercontrol && !placement)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position, 0.3f, shouldFreeHandle);
            leftBlockActive = true;
            movedOnce = true;
        }
        if (message == "right" && !playercontrol && !placement)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position + new Vector3((float)2.5, 0, 0), 0.3f, shouldFreeHandle);
            leftBlockActive = false;
            movedOnce = true;
        }
        if(message == "confirm" && !playercontrol && movedOnce && !placement)
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
        if(message == "place" && playercontrol)
        {
            if(Playfield.isValidPlacement(activeBlock)){
            placement = true;
            playercontrol = false;
            Playfield.roundAndPlaceBlock(activeBlock);
            await meHandle.MoveToPosition(activeBlock.transform.position, 0.3f, shouldFreeHandle);
            } else {await speechOut.Speak("You cannot place the block here.");}
        }
        if(message == "confirm" && placement)
        {
            activeBlock.name = "PlacedBlock" + SpawnManager.waveNumber;
            activeBlock.transform.parent = null; //detach Block
            placement = false;
            Playfield.confirmBlock(activeBlock);
            Playfield.checkRows();
            SpawnManager.spawnWavePls = true;
            await meHandle.MoveToPosition(SpawnerLeft.transform.position, 0.3f, shouldFreeHandle);
            transform.position = SpawnerLeft.transform.position;
        }
        if(message == "abort" && placement)
        {
            placement = false;
            playercontrol = true;
            meHandle.Free();
        }
    }
}
