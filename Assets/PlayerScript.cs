using System;
using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private GameObject Ball;
    private Collider m_Collider;
    public float forceMultiplier = 1f;  //Multiplier to increase hitstrength
    public float upForce = 5f;
    private float velocity = 0f;     //Stores the velocity of the moving club
    private BallAudio soundEffects;

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
        Vector3 show_goal = GameObject.Find("Goal").transform.position - GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position);
        //GameObject.Find("Panto").GetComponent<UpperHandle>().
        ReadyToHit();
    }

    private void FixedUpdate()
    {
        //Move Club to Handle Position
        StartCoroutine(MoveOverSpeed(GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position), 100));
        //Rotate Club according to Handle Position
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            GameObject.Find("Panto").GetComponent<UpperHandle>().GetRotation(),
            transform.eulerAngles.z
            );
        velocity = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Ball)
        {
            Debug.Log("Hitting the Ball. Collider disabled");
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
            m_Collider.enabled = false;
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
            Rigidbody rb = Ball.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude > 0.02)   //Check if ball is moving
            {
                return false;
            }
            // Ball is not moving anymore:
            rb.velocity = Vector3.zero;     //Balls velocity set to 0.
            Debug.Log("Collider enabled.");
            m_Collider.enabled = true;      //Enable Club to make next hit.
        }
        return true;
    }
}
