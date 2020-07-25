using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DualPantoFramework;
using UnityEngine.Rendering;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public int level_num = 0;
    bool boundariesMoving = false;
    public UIManager uiManager;
    public GameObject tablePrefab;
    public GameObject sofaPrefab;
    public GameObject tvPrefab;
    public GameObject closetPrefab;
    public GameObject chairPrefab;
    public GameObject shelfPrefab;
    public GameObject doorPrefab;
    public GameObject windowPrefab;

    public GameObject tracedObject;
    public GameObject createdObject;

    Bounds roomBounds;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    Level level;
    SpeechIn speechIn;
    SpeechOut speechOut;
    WindowSounds soundEffectsW;
    DoorSounds soundEffectsD;

    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "move", KeyCode.M },
        { "place", KeyCode.P },
        { "trace", KeyCode.T },
        { "drop", KeyCode.I },
        { "delete", KeyCode.D },
        { "get table", KeyCode.Alpha0 },
        { "get wardrobe", KeyCode.Alpha4 },
        { "get sofa", KeyCode.None },
        { "get TV", KeyCode.None },
        { "get shelf", KeyCode.None },
        { "get chair", KeyCode.None },

        { "sofa", KeyCode.Alpha1 },
        { "table", KeyCode.Alpha2 },
        { "closet", KeyCode.Alpha3 },
        { "TV", KeyCode.Alpha4 },
    };
 
    GameObject selectedObject = null;

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();
        level = GetComponent<Level>();
    }

    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        soundEffectsW = GetComponent<WindowSounds>();
        speechIn.StartListening(commands.Keys.ToArray());
        roomBounds = GameObject.Find("Room").GetComponent<BoxCollider>().bounds;
        Introduction();
    }

    async void Update()
    {
        SelectedObjectUpdate();
        if(this.selectedObject != null)
        {
            Bounds objectBounds = selectedObject.GetComponent<BoxCollider>().bounds;
            Vector3 currentPos = upperHandle.GetPosition();
            bool shouldMove = false; 
            if(objectBounds.min.x < roomBounds.min.x)
            {
                currentPos = new Vector3(currentPos.x - (objectBounds.min.x - roomBounds.min.x), currentPos.y, currentPos.z);
                shouldMove = true;

            }
            if (objectBounds.min.z < roomBounds.min.z)
            {
                currentPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - (objectBounds.min.z - roomBounds.min.z));
                shouldMove = true;

            }
            if (objectBounds.max.x > roomBounds.max.x)
            {
                currentPos = new Vector3(currentPos.x, currentPos.y, currentPos.x - (objectBounds.max.x - roomBounds.max.x));
                shouldMove = true;

            }
            if (objectBounds.max.z > roomBounds.max.z)
            {
                currentPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - (objectBounds.max.z - roomBounds.max.z));
                shouldMove = true;

            }
            if (shouldMove == true && this.boundariesMoving == false)
            {
                this.boundariesMoving = true;
                await upperHandle.MoveToPosition(currentPos, 1f);
                this.boundariesMoving = false;
            }
        }
    }
    private void SelectedObjectUpdate()
    {
        if (selectedObject == null) return;
        selectedObject.transform.position = upperHandle.GetPosition() + new Vector3(0, 0.2f, 0);
    }

    Dictionary<string, KeyCode> CommandDictFor(string[] keys)
    {
        Dictionary<string, KeyCode> ret = new Dictionary<string, KeyCode>();
        foreach (string key in keys)
        {
            ret[key] = this.commands[key];
        }
        return ret;
    }

    Dictionary<string, KeyCode> CommandDictFor(string key)
    {
        Dictionary<string, KeyCode> ret = new Dictionary<string, KeyCode>();
        ret[key] = this.commands[key];
        return ret;
    }
    async void Introduction()
    {
        await speechOut.Speak("Welcome to Interior Design");
        await Task.Delay(1000);
        RegisterColliders();
        await speechOut.Speak("You're in a room.");
        await level.PlayIntroduction();
        await speechOut.Speak("Feel for yourself.");
        await Task.Delay(1500);
        levelOne();

    }

    async void levelOne()
    {
        await speechOut.Speak("Right now there's also a sofa in the room. Try selecting it by saying 'sofa'.");
        await speechIn.Listen(this.CommandDictFor("sofa"));
       if (selectedObject = GameObject.Find("Sofa"))
        {
            await speechOut.Speak("Cool! now to feel its shape say 'trace'");
            await speechIn.Listen(this.CommandDictFor("trace"));
            if (tracedObject = GameObject.Find("Sofa"))
            {
                await speechOut.Speak("Great! That sofa would fit perfectly on the corner in front of the TV. Say 'move' to move the sofa around, and 'drop' when you're done.");
            }

        }
        await speechIn.Listen(this.CommandDictFor("move"));
        await speechIn.Listen(this.CommandDictFor("drop"));
        levelTwo();
    }

    async void levelTwo()
    {
        GameObject window = Instantiate(windowPrefab, new Vector3(0, 0.2f, -10), Quaternion.identity); // wip set the right position 
        await speechOut.Speak("Well done, now is gonna be really confy while watching TV. We still need a small table. Say 'get table'");
        await speechIn.Listen(this.CommandDictFor("get table")); //wip
        await speechOut.Speak("It would be really nice to have it in front of the window. Move it around and drop the table in front of the window");
        this.selectObject(GameObject.Find("Table"));
        await speechIn.Listen(this.CommandDictFor("move"));
        await speechIn.Listen(this.CommandDictFor("drop"));
        levelThree();
    }

    async void levelThree()
    {
        GameObject window = Instantiate(doorPrefab, new Vector3(0, 0.2f, -10), Quaternion.identity); //wip set the right position
        if (selectedObject = null)
        {
            await speechOut.Speak("Let's get a wardrobe now. Say 'get wardrobe'. ");
            await speechIn.Listen(this.CommandDictFor("createWardrobe"));
            await speechOut.Speak("Place it on the right wall of the room, but be careful to not place it in front of the door.");
        }
        await speechIn.Listen(commands);
        await speechIn.Listen(this.CommandDictFor("drop"));
        levelFour();
    }

    async void levelFour()
    {
        await speechOut.Speak("Perfect!, but the doors of the waredorbe still pointing the wall. Let's fix that! try selecting the warderobe again and say rotate it");
        await speechIn.Listen(commands);
        await speechIn.Listen(this.CommandDictFor("drop"));
        levelFive();
    }

    async void levelFive()
    {
        if (selectedObject = null)
        {
            await speechOut.Speak("Great! now you have organized your livingroom!, if you want to delete any object just say 'delete' and the object name." +
                "if you want to delete all objects in the room say 'delete all'" +
                "if you want now you can keep adding objects or delete them and start over. "+
                "other objects that you can get are a chair and a shelf. Have fun!");
        }
    }

    void RegisterColliders()
    {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in colliders)
        {
            Debug.Log(collider);
            collider.CreateObstacle();
            collider.Enable();
        }
    }

   
    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        /// challenge: when other has tag "Enemy" and we have a powerup
        /// get the enemyRigidbody and push the enemy away from the player
        if (other.CompareTag("Window"))
        {
            soundEffectsW.playWindowClip();
        }
        if (other.CompareTag("Window"))
        {
            soundEffectsD.playDoorClip();
        }
    }

    async void selectObject(GameObject obj)
    {
        this.selectedObject = obj;
        Task[] moveHandles = {
            upperHandle.MoveToPosition(obj.transform.position, 0.2f, false),
            lowerHandle.SwitchTo(obj, 0.2f),
            speechOut.Speak("selected " + obj.name)
        };
        await Task.WhenAll(moveHandles);

    }

    async void deleteObject(GameObject obj)
    {
        Destroy(obj);
        await speechOut.Speak(obj + "deleted");
    }

    async void onRecognized(string message)
    {
        Debug.Log("SpeechIn recognized: " + message);
        if(message == "delete all")
        {
            //delete all elements on the room wip
        }
        if (this.selectedObject != null)
        {
            switch (message)
            {
                case "move":
                    if (selectedObject.tag == "Movable")
                    {
                        upperHandle.Free();
                    }
                    else
                    {
                        await speechOut.Speak("Unable to move this object.");
                    }
                    break;
                case "place":
                    selectObject(selectedObject);
                    break;
                case "trace":
                    await level.TraceObject(selectedObject);
                    await lowerHandle.SwitchTo(selectedObject, 0.2f);
                    tracedObject = selectedObject;
                    break;
                case "drop":
                    selectedObject = null;
                    upperHandle.Free();
                    lowerHandle.Free(); 
                    break;
                case "delete":
                    deleteObject(selectedObject);
                    selectedObject = null;
                    break;
                case "delete all": //wip 
                    break;
            }
        }
        else 
        {
            switch (message)
            {
                case "get sofa":
                    GameObject newSofa = Instantiate(sofaPrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("Sofa"));
                    await speechOut.Speak("new sofa created"); break;
                case "get table": GameObject newTable = Instantiate(tablePrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("Table"));
                    await speechOut.Speak("new table created"); break;
                case "get closet": GameObject newCloset = Instantiate(tablePrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("Closet"));
                    await speechOut.Speak("new wardrobe created"); break;
                case "get TV": GameObject newTV = Instantiate(tablePrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("TV"));
                    await speechOut.Speak("new TV created"); break;
                case "get shelf":
                    GameObject newShelf = Instantiate(shelfPrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("Shelf"));
                    await speechOut.Speak("new shelf created"); break;
                case "get chair":
                    GameObject newChair = Instantiate(chairPrefab, new Vector3(0, 0.2f, -10), Quaternion.identity);
                    this.selectObject(GameObject.Find("Chair"));
                    await speechOut.Speak("new chair created"); break;

                case "sofa": this.selectObject(GameObject.Find("Sofa")); break;
                case "table": this.selectObject(GameObject.Find("Table")); break;
                case "closet": this.selectObject(GameObject.Find("Closet")); break;
                case "TV": this.selectObject(GameObject.Find("TV")); break;
                case "shelf": this.selectObject(GameObject.Find("Shelf")); break;
                case "chair": this.selectObject(GameObject.Find("Chair")); break;
            }
        }
    }

    public void OnApplicationQuit()
    {
        speechOut.Stop(); // [Windows] do not remove this line.
        speechIn.StopListening(); // [macOS] do not delete this line!
    }
}
