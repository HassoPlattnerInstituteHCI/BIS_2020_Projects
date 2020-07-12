using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

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
        if (collision.gameObject.name == "Lungs")
        {
            await speechOut.Speak("Congratulations, you reached the lungs");
            currentTarget++;
        }
        else if (collision.gameObject.name == "Liver" || collision.gameObject.name == "Stomach" || collision.gameObject.name == "Lungs")
        {
            await speechOut.Speak("Sorry, that is not the organ you are looking for");
        }
        else if (collision.gameObject.name == "Ulcer0" || collision.gameObject.name == "Ulcer1" || collision.gameObject.name == "Ulcer2" || collision.gameObject.name == "Ulcer3")
        {
            await speechOut.Speak("Ouch, you touched an open wound. That is the end of the game.");
        }

        Debug.Log(collision.gameObject.name);

        /* Debug.Log(collision.gameObject.name);
        Debug.Log(currentTarget); */
    }
}
