using System;
using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using SpeechIO;

public class PlayerScript : MonoBehaviour
{

    public bool allowMovement = true;
    public bool disableHit = false;
    private GameObject Ball;
    private Collider m_Collider;
    public float forceMultiplier = 1f;  //Multiplier to increase hitstrength
    public float upForce = 5f;
    private float velocity = 0f;     //Stores the velocity of the moving club
    private BallAudio soundEffects;
    private int hitCount = 0;
    public SpeechOut speechOut = new SpeechOut();
    private Vector3 previousPosition;   //To calculate velocity of club.

    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.FindGameObjectWithTag("ball");
        m_Collider = GetComponent<Collider>();
        soundEffects = Ball.GetComponent<BallAudio>();
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position);
        
        //GameObject.Find("Panto").GetComponent<UpperHandle>().
        ReadyToHit();
    }

    private void FixedUpdate()
    {
        //Move Club to Handle Position
        if (allowMovement)
        {
            StartCoroutine(MoveOverSpeed(GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position), 100));
            //Rotate Club according to Handle Position
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                GameObject.Find("Panto").GetComponent<UpperHandle>().GetRotation(),
                transform.eulerAngles.z
                );
            velocity = (transform.position - previousPosition).magnitude / Time.deltaTime;
        }
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Ball && m_Collider.enabled)
        {
            Debug.Log("Hitting the Ball, Collider disabled.");
            hitCount++;
            Vector3 shotDir = new Vector3(0, 0, 0);
            float angle = transform.eulerAngles.y * Mathf.Deg2Rad;
            shotDir.x = Mathf.Cos(angle);
            shotDir.z = -Mathf.Sin(angle);
            Vector3 dir = Ball.transform.position - transform.position;
            dir.y = 0;
            Vector3 up = new Vector3(0, upForce, 0);
            Debug.Log(shotDir);
            Ball.GetComponent<Rigidbody>().AddForce(shotDir.normalized * forceMultiplier * velocity);
            //Ball.GetComponent<Rigidbody>().AddForce(up);
            soundEffects.PlayClubHit();
            soundEffects.PlayRolling(1f);
            m_Collider.enabled = false;
            Debug.Log("End of Player Trigger Event");
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual bool ReadyToHit()
    {
        if (!m_Collider.enabled)
        {
            if (disableHit)
            {
                return false;
            }
            Rigidbody rb = Ball.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude > 0.02)   //Check if ball is moving
            {
                return false;
            }
            Debug.Log("Ball is not moving:");
            // Ball is not moving anymore:
            rb.velocity = Vector3.zero;     //Balls velocity set to 0.
            soundEffects.StopRolling();
            //int nexthit = hitCount + 1;
            //VoiceOut("Waiting for hit "+ nexthit);
            soundEffects.PlayReadyToHit();
            m_Collider.enabled = true;      //Enable Club to make next hit.
            Debug.Log("Collider enabled.");
        }
        return true;
    }

    private async void VoiceOut(string message)
    {
        speechOut.Stop();
        await speechOut.Speak(message);
    }
}
