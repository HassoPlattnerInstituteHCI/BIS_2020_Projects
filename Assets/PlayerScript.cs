using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private GameObject Ball;
    private Collider m_Collider;
    public float forceApplied = 50;
    public float upForce = 5f;
    private BallAudio soundEffects;

    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.FindGameObjectWithTag("ball");
        m_Collider = GetComponent<Collider>();
        soundEffects = Ball.GetComponent<BallAudio>();
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
        StartCoroutine(MoveOverSpeed(GameObject.Find("Panto").GetComponent<UpperHandle>().HandlePosition(transform.position), 100));
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Ball)
        {
            Debug.Log("Hitting the Ball. Collider disabled");
            Vector3 dir = Ball.transform.position - transform.position;
            dir.y = 0;
            Vector3 up = new Vector3(0, upForce, 0);
            Ball.GetComponent<Rigidbody>().AddForce(dir.normalized * forceApplied);
            Ball.GetComponent<Rigidbody>().AddForce(up);
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
            if (rb.velocity.magnitude > 0.01)
            {
                return false;
            }
            Debug.Log("Collider enabled.");
            m_Collider.enabled = true;
        }
        return true;
    }
}
