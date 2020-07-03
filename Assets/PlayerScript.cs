using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private GameObject Ball;
    private Collider m_Collider;
    public float forceApplied = 50;
    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.FindGameObjectWithTag("ball");
        m_Collider = GetComponent<Collider>();
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
        MoveOverSeconds(GameObject.Find("Player"), GameObject.Find("Panto").GetComponent<UpperHandle>().GetPosition(), 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("ball"))
        {
            Debug.Log("Ball getroffen, Collider disabled.");
            //Ball.GetComponent<Rigidbody>().AddForce(0, forceApplied, 0);
            // Calculate Angle Between the collision point and the player
            //Vector3 dir = collision.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            //dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
           // Ball.GetComponent<Rigidbody>().AddForce(dir * forceApplied);
            m_Collider.enabled = false;
        }
    }

    public IEnumerator MoveOverSpeed(Vector3 end, float speed)
    {
        // speed should be 1 unit per second
        while (transform.position != end)
        {
            Debug.Log("Nicht am Ziel");
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }

    protected virtual bool ReadyToHit()
    {
        if (!m_Collider.enabled)
        {
            Rigidbody rb = Ball.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude > 0)
            {
                return false;
            }
            Debug.Log("Collider enabled.");
            m_Collider.enabled = true;
        }
        return true;
    }
}
