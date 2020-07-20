using System.Threading.Tasks;
using SpeechIO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

////TODO Level 5
//// HitCount + reset in safehouse
//Cashcount
/// Danny talks, explains level
// on 4th hit, hearbeat start
//on 5th hit,sirens start, more cash for hits


public class GameManager : MonoBehaviour
{
    public float spawnSpeed = 1f;
    public bool introduceLevel = true;
    public GameObject player;
    public GameObject enemy;
    public EnemyConfig[] enemyConfigs;
    public Transform playerSpawn;
    public Transform enemySpawn;
    public int level = 0;
    public int trophyScore = 10000;
    public UIManager uiManager;
    public int currentLevel;
    private TelephoneSoundEffect telephoneSounds;
    private PlayerSoundEffect playerSounds;

    GameObject phoneBox;
    GameObject bat;

    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    SpeechIn speechIn;
    SpeechOut speechOut;
    int playerScore = 0;
    int enemyScore = 0;
    int gameScore = 0;
    float totalTime = 0;
    float levelStartTime = 0;
    List<Vector3> listOfSpawnPositions;
    List<int> spawnUsed = new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 0};
    public int hitCount = 0;
    public int cash = 0;

    public bool currentObjectiveReached = false;
    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };


    void Awake()
    {
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
        speechOut = new SpeechOut();

        if (level < 0 || level >= enemyConfigs.Length)
        {
            Debug.LogWarning($"Level value {level} < 0 or >= enemyConfigs.Length. Resetting to 0");
            level = 0;
        }
    }

    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        

        listOfSpawnPositions = createAHoleSpawns();

        

        //uiManager.UpdateUI(playerScore, enemyScore);

        Introduction();
    }

    async void Introduction() //Speech: Introduce Me-Handle = Move. - Go to telephone
    {

        bat = GameObject.Find("Bat");
        bat.SetActive(false);
        
        await speechOut.Speak("Use the upper handle to move your character. Spawning Player");        
        await ResetGame();
        player = GameObject.Find("Player");
        playerSounds = player.GetComponent<PlayerSoundEffect>();

        playerSounds.startHitZeroMusic();

        //await Task.Delay(1000);
        RegisterColliders();

        if (introduceLevel)
        {
            await StartLevel1();
        }

        
    }

    async Task StartLevel1()
    {
        currentLevel = 1;
        await speechOut.Speak("Pick up the phone");  
        
        transform.rotation = Quaternion.Euler(0, upperHandle.GetRotation(), 0);

        phoneBox = GameObject.Find("TelephoneBox1");   
        telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();        
        telephoneSounds.startPhoneRing(phoneBox);    
        
    }

    public async Task StartLevel2(){
        currentLevel = 2;
        playerSpawn.position = phoneBox.transform.position;
        phoneBox = GameObject.Find("TelephoneBox2");

        bat.SetActive(true);
        lowerHandle.SwitchTo(bat, 0.1f);        

        telephoneSounds = phoneBox.GetComponent<TelephoneSoundEffect>();
        telephoneSounds.startPhoneRing(phoneBox);       

    }

    public async Task StartLevel3(){
        currentLevel = 3;
        playerSpawn.position = phoneBox.transform.position;
        playerSounds.startSirens();

    }

    public async Task StartLevel4(){
        currentLevel = 4;

        GameObject safeHouse = GameObject.Find("SafeHouse");
        playerSpawn.position = safeHouse.transform.position;

        spawnAHoles(5);

         


    }

    public async Task StartLevel5(){
        currentLevel = 5;

        GameObject safeHouse = GameObject.Find("SafeHouse");
        playerSpawn.position = safeHouse.transform.position;

    }

     public async Task StartLevel6(){
        currentLevel = 6;

        GameObject safeHouse = GameObject.Find("SafeHouse");
        playerSpawn.position = safeHouse.transform.position;

    }

    public async Task spawnAHoles(int num){
        if(num>sumOfFreePositions()){
            Debug.LogError("Tried to spawn more AHoles than spanwpositions available");
            num = sumOfFreePositions();
        }

        for(int i = 0; i<num; i++){
            int rInt;
            do{
                System.Random r = new System.Random();
                rInt = r.Next(0, listOfSpawnPositions.Count-1);
            } while(spawnUsed[rInt]==1);
            
            GameObject thisAHole = (GameObject) Instantiate(Resources.Load("AHolePrefab"), listOfSpawnPositions[rInt], Quaternion.identity);
            AHoleSoundEffect aHoleSounds = thisAHole.GetComponent<AHoleSoundEffect>();
            aHoleSounds.startBlaBla();
            spawnUsed[rInt] = 1;
        }
    }

    public async Task deleteAHole(GameObject victim){
        //victim.active = false;
        Destroy(victim);
        int pos = 0;
        while(victim.transform.position != listOfSpawnPositions[pos]){
            pos++;
        }
        spawnUsed[pos] = 0;
        
    }

    

    

    void RegisterColliders() {
        PantoCollider[] colliders = FindObjectsOfType<PantoCollider>();

    //TODO: ADD all obstacles and walls to the game

        foreach (PantoCollider collider in colliders)
        {
            Debug.Log(collider);
            collider.CreateObstacle();
            collider.Enable();
        }
    }

    /// <summary>
    /// Starts a new round.
    /// </summary>
    /// <returns></returns>


    public async Task ResetGame()
    {
        
        player.transform.position = playerSpawn.position;
        await upperHandle.MoveToPosition(player.transform.position, 0.2f, true);

        upperHandle.Free();

        cash = 0;
        Debug.Log("cash: " + cash);

        player.SetActive(true);
        GameObject camera = GameObject.Find("Main Camera");
        camera.GetComponent<AudioListener>().enabled = false;
        levelStartTime = Time.time;
    }

    async void onRecognized(string message)
    {
        Debug.Log("SpeechIn recognized: " + message);
    }

    public void OnApplicationQuit()
    {
        speechOut.Stop(); //Windows: do not remove this line.
        speechIn.StopListening(); // [macOS] do not delete this line!
    }

    /// <summary>
    /// Ends the round by disabling player and enemy, updating UI, calculating
    /// game score and eventually ending the game.
    /// </summary>
    /// <param name="defeated"></param>

    /// <summary>
    /// Ends the game.
    /// </summary>
    /// <returns></returns>
    async Task GameOver()
    {
        await speechOut.Speak("Congratulations.");

        await speechOut.Speak("Thanks for playing GTA2. Say quit when you're done.");
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "quit", KeyCode.Escape } });

        Application.Quit();
    }

    /// <summary>
    /// Calculates the game score by healt, level complete time and enemy
    /// difficulty.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public List<Vector3> createAHoleSpawns(){
        List<Vector3> listOfSpawnPositions = new List<Vector3>();        
        listOfSpawnPositions.Add(new Vector3(7,0,-8));
        listOfSpawnPositions.Add(new Vector3(5,0,-11));
        listOfSpawnPositions.Add(new Vector3(3,0,-10));
        listOfSpawnPositions.Add(new Vector3(-1,0,-11));
        listOfSpawnPositions.Add(new Vector3(-1,0,-5));
        listOfSpawnPositions.Add(new Vector3(2,0,-6));
        listOfSpawnPositions.Add(new Vector3(-4,0,-10));
        listOfSpawnPositions.Add(new Vector3(-6,0,-9));
        listOfSpawnPositions.Add(new Vector3(-1,0,-8));
        return listOfSpawnPositions;
    }
    public int sumOfFreePositions(){
        int free = 0;
        foreach(int spawn in spawnUsed){
            if(spawn == 0) free ++;
        }
        return free;
    }
}