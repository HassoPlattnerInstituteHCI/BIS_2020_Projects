using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

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
            GameObject.Find("Liver"),
            GameObject.Find("Heart"),
            GameObject.Find("Stomach"),
            GameObject.Find("Lungs")};
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
        if (collision.gameObject.name == targets[currentTarget].name)
        {
            await speechOut.Speak("Congratulations, you reached the " + targets[currentTarget].name);
            currentTarget++;
        }
        else
        {
            await speechOut.Speak("Sorry, that is not the organ you are looking for");
        }

        Debug.Log(collision.gameObject.name);

        if (currentTarget == targets.Length)
        {
            await speechOut.Speak("You completed level 2");
        }
    }
}
