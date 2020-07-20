using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
using UnityEngine.SceneManagement;

public class PlayerLogic2 : MonoBehaviour
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

    private int currentTarget = 0;
    private GameObject[] targets;

    SpeechOut speechOut;

    private void Awake()
    {
        speechOut = new SpeechOut();

        targets = new GameObject[] {
            GameObject.Find("Lungs"),
            GameObject.Find("Heart"),
            // GameObject.Find("Stomach"),
            GameObject.Find("Liver")};
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

        if (name.Contains("Fill") || name.Contains("group"))
        {

        }
        else
        {
            await speechOut.Speak(collision.gameObject.name);
        }

        if (name == targets[currentTarget].name)
        {
            currentTarget++;
            if (currentTarget < targets.Length)
            {
                await speechOut.Speak("Feel the " + targets[currentTarget].name + "next.");
            }
            else
            {
                await speechOut.Speak("Alright, let's make it harder.");
                /*UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();*/
                AsyncOperation async = SceneManager.LoadSceneAsync(1);
                async.allowSceneActivation = true;
            }
        }
    }
}
