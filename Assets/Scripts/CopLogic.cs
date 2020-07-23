 
using UnityEngine;
using UnityEngine.AI;
using DualPantoFramework;
using System.Collections;

public class CopLogic : MonoBehaviour
{
    public Transform target;
    public LayerMask layerMask;
    public float stopDistance = 10;
    public CopConfig config;

    bool foundPlayer = false;
    float timeToFind;
    Vector3 lastSeenPosition;
    NavMeshAgent agent;

    GameObject player;
    GameManager gameManager;
    CopSoundEffect copSounds;
    public int healthLeft = 3;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;

        copSounds = GetComponent<CopSoundEffect>();
        copSounds.playCopCarArrived();

        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }

    void OnEnable()  
    {   

        player = GameObject.Find("Player");
        config = (CopConfig) ScriptableObject.CreateInstance("CopConfig");
        copSounds = GetComponent<CopSoundEffect>();
        copSounds.playCopCarArrived();

        //Makes Cop Start going for Player only after he left his car
        StartCoroutine(waiter(2));
        
        
    }

    IEnumerator waiter(int time){
        yield return new WaitForSeconds(time);
        target = player.transform;
        copSounds.startCopsRadioTalk();

        if (config.attackPlayerAtStart)
        {
            lastSeenPosition = target.position;
            foundPlayer = true;
        }
    }

    void Update()
    {
        if (config.CSGoPlayer)
        {
            AimbotMode();
        }

        agent.SetDestination(lastSeenPosition);
        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, config.turnSpeed);

        if (healthLeft <= 0) {
            Destroy(this.gameObject);
            PlayerSoundEffect playersounds = player.GetComponent<PlayerSoundEffect>();
            playersounds.playCopTurnsCash();
            player.GetComponent<PlayerLogic>().resetTimer();
            gameManager.copsKilled += 1;
            gameManager.cash += (int)(20*(1 + gameManager.copsKilled*0.5 + gameManager.hitCount*0.1));
            Debug.Log("Cash: " + gameManager.cash);
            gameManager.makeWaveOfCopsArriveAfterTime(5.0f, 2);
        }
        if (healthLeft < 3) Debug.Log(healthLeft + " cop health left");
    }

    /// <summary>
    /// Always knows where the player is.
    /// </summary>
    void AimbotMode()
    {
        lastSeenPosition = target.position;
    }

}
