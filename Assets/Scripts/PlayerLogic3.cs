using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using UnityEngine.SceneManagement;

public class PlayerLogic3 : MonoBehaviour
{
    private PantoHandle upperHandle;

    AudioSource audioSource;
    public AudioClip heartbeatClip;

    public int startBPM = 60;
    public int endBPM = 220;
    float bpmCoefficient;
    public float bps = 1;
    float nextHeartbeat;
    Health health;

    SpeechOut speechOut;
    bool strike = false;

    private void Awake()
    {
        speechOut = new SpeechOut();
    }

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        /* health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2); */
    }

    void Update()
    {
        transform.position = upperHandle.HandlePosition(transform.position);

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

    private async void OnCollisionEnter(Collision collision)
    {
        string name = collision.gameObject.name;

        if ("Stomach" == collision.gameObject.name)
        {
            await speechOut.Speak("Well done! Now, we'll try to remove an organ.");
            /*UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();*/
            AsyncOperation async = SceneManager.LoadSceneAsync(1);
            async.allowSceneActivation = true;
        }
        else if (!collision.gameObject.name.Contains("group"))
        {
            Debug.Log(name);
            if (strike)
            {
                await speechOut.Speak("Oh no, you messed up! Game over!");
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                await speechOut.Speak("Be careful! You got one more chance.");
                strike = true;
            }
        }
        
    }
}
