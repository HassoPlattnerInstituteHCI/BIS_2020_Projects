using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

public class Player : MonoBehaviour
{
    private PantoHandle meHandle;
    bool movementStarted = false;
    bool playercontrol = false;
    public bool shouldFreeHandle;
    public float movementspeed = 0.2f;
    public GameObject SpawnerLeft;
    public GameObject SpawnerRight;
    SpeechIn speechIn;
    SpeechOut speechOut;
    
    float lastFall = 0;
    // Start is called before the first frame update

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized);
        speechOut = new SpeechOut();
        
    }

    async void Start()
    {
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        //GameObject SpawnerRight = GameObject.Find("SpawnerRight");
        //GameObject SpawnerLeft = GameObject.Find("SpawnerLeft");
        await meHandle.SwitchTo(gameObject, 0.4f);
        //movementStarted = true;
        await speechOut.Speak("Welcome to Tetris Panto Edition");
        //Is th is the right place for it? Will need it in Update to work also for the next waves of blocks
        speechIn.StartListening(new string[]{"left", "right", "confirm"});
    }

    // Update is called once per frame
    void Update()
    {
        if (playercontrol)
        {
            //transform.position = meHandle.HandlePosition(transform.position);
                    //From here on movement via Keyboard arrows for now. Need to couple with Me-Handle movements.
        // Move Left
    if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        // Modify position
        transform.position += new Vector3((float)-0.5, 0, 0);
       
        // See if valid
        if (isValidGridPos())
            // It's valid. Update grid.
            updateGrid();
        else
            // It's not valid. revert.
            transform.position += new Vector3((float)0.5, 0, 0);
    }

    // Move Right
    else if (Input.GetKeyDown(KeyCode.RightArrow)) {
        // Modify position
        transform.position += new Vector3((float)0.5, 0, 0);
       
        // See if valid
        if (isValidGridPos())
            // It's valid. Update grid.
            updateGrid();
        else
            // It's not valid. revert.
            transform.position += new Vector3((float)-0.5, 0, 0);
    }

    // Rotate
    else if (Input.GetKeyDown(KeyCode.UpArrow)) {
        transform.Rotate(0, -90, 0);
       
        // See if valid
        if (isValidGridPos())
            // It's valid. Update grid.
            updateGrid();
        else
            // It's not valid. revert.
            transform.Rotate(0, 90, 0);
    }

    // Move Downwards and Fall
    else if (Input.GetKeyDown(KeyCode.DownArrow)) {
        // Modify position
        transform.position += new Vector3(0, 0, (float)-0.5);

        // See if valid
        if (isValidGridPos()) {
            // It's valid. Update grid.
            updateGrid();
        } else {
            // It's not valid. revert.
            transform.position += new Vector3(0, 0, (float)0.5);

            // Clear filled horizontal lines
            Playfield.deleteFullRows();

            // Spawn next Group
            //FindObjectOfType<Spawner>().spawnNext();

            // Disable script
            enabled = false;
        }

    }
        }
        
        if (movementStarted)
        {

        }

    }
    async void onRecognized(string message)
    {
        if(message == "left" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position, 0.3f, shouldFreeHandle);
        }
        else if(message == "right" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerRight.transform.position, 0.3f, shouldFreeHandle);
        }
        else if(message == "confirm" && !playercontrol)
        {
            //TODO: Delete other block
            playercontrol = true;
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
