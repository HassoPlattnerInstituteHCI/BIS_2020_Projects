using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Security;
using UnityEditorInternal;
using UnityEditor;

public class Player : MonoBehaviour
{
    private PantoHandle meHandle;
    GameObject meHandlePrefab;
    bool movementStarted = false;
    bool playercontrol = false;
    bool chooseMode = true;
    bool leftBlockActive = false;
    bool movedOnce = false;
    public bool shouldFreeHandle;
    public float movementspeed = 0.2f;
    public GameObject SpawnerLeft;
    public GameObject SpawnerRight;
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
        speechIn = new SpeechIn(onRecognized, new string[] { "repeat", "left", "right", "confirm" });
        speechIn.StartListening();
        meHandlePrefab = GameObject.Find("MeHandlePrefab(Clone)");
    }

    // Update is called once per frame
    async void Update()
    {
       
            if (playercontrol) {
            //transform.position = meHandle.HandlePosition(transform.position);
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
                transform.Rotate(0, -90, 0);
            }

            else if (Input.GetKeyDown(KeyCode.Return)) {
                //playercontrol = false;
                Playfield.deleteFullRows();
            }
        }

        

        if (movementStarted)
        {

        }

    }
    
    async void onRecognized(string message)
    {
        Debug.Log("[" + this.GetType() + "]:" + message);
        switch (message)
        {
            case "repeat":
                await speechOut.Repeat();
                break;
        }
        if (message == "left" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position, 0.3f, shouldFreeHandle);
            leftBlockActive = true;
            movedOnce = true;
        }
        if (message == "right" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position + new Vector3((float)2.5, 0, 0), 0.3f, shouldFreeHandle);
            leftBlockActive = false;
            movedOnce = true;
        }
        if(message == "confirm" && !playercontrol && movedOnce)
        {
            if(leftBlockActive) {
                Destroy(SpawnManager.blockRight);
                SpawnManager.blockLeft.transform.SetParent(transform);
            } else {Destroy(SpawnManager.blockLeft); SpawnManager.blockRight.transform.SetParent(transform);}

            playercontrol = true;
            chooseMode = false;
        }
    }



    bool isValidGridPos() {        
    foreach (Transform child in transform) {
        Vector3 v = Playfield.roundVec3(child.position);

        // Not inside Border?
        if (!Playfield.insideBorder(v))
            return false;

        // Block in grid cell (and not part of same group)?
        if (Playfield.grid[(int)v.x, (int)v.y] != null &&
            Playfield.grid[(int)v.x, (int)v.y].parent != transform)
            return false;
    }
    return true;
    }

    void updateGrid() {
    // Remove old children from grid
    for (int y = 0; y < Playfield.h; ++y)
        for (int x = 0; x < Playfield.w; ++x)
            if (Playfield.grid[x, y] != null)
                if (Playfield.grid[x, y].parent == transform)
                    Playfield.grid[x, y] = null;

    // Add new children to grid
    foreach (Transform child in transform) {
        Vector3 v = Playfield.roundVec3(child.position);
        Playfield.grid[(int)v.x, (int)v.y] = child;
    }        
    }
}
