﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerLogic4 : MonoBehaviour
{
    private PantoHandle upperHandle;
    private PantoHandle lowerHandle;

    AudioSource audioSource;
    public AudioClip heartbeatClip;

    public int startBPM = 60;
    public int endBPM = 220;
    float bpmCoefficient;
    public float bps = 1;
    float nextHeartbeat;
    Health health;

    SpeechOut speechOut;
    SpeechIn speechIn;
    private int lives = 2;
    private GameObject scalpel;
    private GameObject heart;
    private GameObject player;
    private bool attached = false;

    Dictionary<string, KeyCode> commands = new Dictionary<string, KeyCode>() {
        { "yes", KeyCode.Y },
        { "no", KeyCode.N },
        { "done", KeyCode.D }
    };

    private void Awake()
    {
        speechOut = new SpeechOut();
        speechIn = new SpeechIn(onRecognized, commands.Keys.ToArray());
    }

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();

        scalpel = GameObject.Find("Scalpel");
        scalpel.SetActive(false);
        Debug.Log(scalpel);
        heart = GameObject.Find("Heart");
        player = GameObject.Find("Player");
        /* health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2); */
    }

    void Update()
    {
        transform.position = upperHandle.HandlePosition(transform.position);

        if (attached)
        {
            heart.transform.position = upperHandle.HandlePosition(heart.transform.position);
            Debug.Log(heart.transform.position);
            Debug.Log(player.transform.position);
        }

        if (heart.transform.position.x > 4.2f && heart.transform.position.z < -13f)
        {
            speechOut.Speak("Great!");
            AsyncOperation async = SceneManager.LoadSceneAsync(1);
            async.allowSceneActivation = true;
        }

        /* if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
        {
            if (nextHeartbeat > bps)
            {
                float bpm = bpmCoefficient * Mathf.Pow(health.healthPoints - health.maxHealth, 2) + startBPM;
                bps = 60f / bpm;
                audioSource.PlayOneShot(heartbeatClip);
                nextHeartbeat = 0;
            }
            else
            {
                nextHeartbeat += Time.deltaTime;
            }
        } */
    }

    async void onRecognized(string message)
    {
        Debug.Log("SpeechIn recognized: " + message);
    }

    private async void OnCollisionEnter(Collision collision)
    {
        string name = collision.gameObject.name;

        if (name.Contains("Fill") || name.Contains("group"))
        {

        }
        else
        {
            if (attached && name == "Heart")
            {
                
            }
            else
            {
                await speechOut.Speak(collision.gameObject.name);
            }
        }

        if ("Heart" == collision.gameObject.name && ! attached)
        {
            if (scalpel.activeSelf)
            {
                attached = true;
                // await speechOut.Speak("Say hand.");
                // await speechIn.Listen(new Dictionary<string, KeyCode>() { { "hand", KeyCode.H } });
                scalpel.SetActive(false);
                await speechOut.Speak("Move to the bin in the bottom right corner.");
                await lowerHandle.MoveToPosition(new Vector3(4.2f, 0f, -13f), 0.3f, false);
            }
            else
            {
                await speechOut.Speak("Now say scalpel.");
                await speechIn.Listen(new Dictionary<string, KeyCode>() { { "scalpel", KeyCode.S } });
                scalpel.SetActive(true);
                await speechOut.Speak("Make a cut to the heart.");
                /*UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();*/
            }
        }
    }
}
