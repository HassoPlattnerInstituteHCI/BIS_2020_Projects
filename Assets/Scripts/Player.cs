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
    SpeechIn speechIn;
    SpeechOut speechOut;
    
    // Start is called before the first frame update

    void Awake()
    {
        speechIn = new SpeechIn(onRecognized);
        speechOut = new SpeechOut();

    }

    async void Start()
    {
        meHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        GameObject SpawnerRight = GameObject.Find("SpawnerRight");
        GameObject SpawnerLeft = GameObject.Find("SpawnerLeft");
        await meHandle.SwitchTo(gameObject, 0.4f);
        //movementStarted = true;
        await speechOut.Speak("Welcome to Tetris Panto Edition");
    }

    // Update is called once per frame
    void Update()
    {
        if (playercontrol)
        {
            transform.position = meHandle.HandlePosition(transform.position);
        }
        
        if (movementStarted)
        {

        }
        
    }
    async void onRecognized(string message)
    {
        if(message == "right" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerLeft.transform.position, 0.3f, shouldFreeHandle);
        }
        else if(message == "right" && !playercontrol)
        {
            await meHandle.MoveToPosition(SpawnerRight.transform.position, 0.3f, shouldFreeHandle);
        }
    }
}
