using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;

public class PlayerLogic : MonoBehaviour
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

    void Start()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        speechOut = new SpeechOut();

        bpmCoefficient = (endBPM - startBPM) / Mathf.Pow(health.maxHealth, 2);
    }

    void Update()
    {
        // Simply connects the player to the upper handles position
        transform.position = upperHandle.HandlePosition(transform.position);

        if (health.healthPoints > 0 && health.healthPoints <= 2 * health.maxHealth / 3)
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
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            health.Heal(25);
            GetComponent<Shooting>().damage = (int)(GetComponent<Shooting>().damage * 1.2);
            //soundEffects.PlayPowerupCollect();
            _ = speechOut.Speak("Powerup found!");
            Destroy(other.gameObject);
        }
    }
}
