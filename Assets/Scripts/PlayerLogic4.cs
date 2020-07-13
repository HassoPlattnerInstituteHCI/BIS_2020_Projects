using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

public class PlayerLogic4 : MonoBehaviour
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

    private bool rightKidneySeparated = false;
    private bool leftKidneySeparated = false;
    private int lives = 2;

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
        if (rightKidneySeparated)
        {
            GameObject kidney = GameObject.Find("KidneyRight");
            kidney.transform.position = GameObject.Find("MeHandlePrefab(Clone)").transform.position;
        }
        if (leftKidneySeparated)
        {
            GameObject kidney = GameObject.Find("KidneyLeft");
            kidney.transform.position = GameObject.Find("MeHandlePrefab(Clone)").transform.position;
        }
    }

    private async void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Int1" || collision.gameObject.name == "Int2")
        {
            Debug.Log(collision.gameObject.transform);
            GameObject kidney = GameObject.Find("KidneyRight");
            kidney.transform.position += Vector3.forward * -1.0f;
            rightKidneySeparated = true;
            await speechOut.Speak("Congratulations, you separated the right kidney.");
        }
        else if (collision.gameObject.name == "Int3" || collision.gameObject.name == "Int4")
        {
            GameObject kidney = GameObject.Find("KidneyLeft");
            kidney.transform.position += Vector3.forward * -1.0f;
            leftKidneySeparated = true;
            await speechOut.Speak("Congratulations, you separated the left kidney.");
        }
        else if (collision.gameObject.name == "Liver" || collision.gameObject.name == "Stomach" || collision.gameObject.name == "Lungs" || collision.gameObject.name == "Heart" || collision.gameObject.name == "KidneyRight" || collision.gameObject.name == "KidneyLeft")
        {
            await speechOut.Speak("You cut in an organ.");
            lives--;
            await speechOut.Speak("You got " + lives + " lives left.");
            if (lives == 0)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
        }

        Debug.Log(collision.gameObject.name);

        /* Debug.Log(collision.gameObject.name);
        Debug.Log(currentTarget); */
    }
}
