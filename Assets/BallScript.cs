using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    private Rigidbody rb;
    private GameObject Ball;
    private BallAudio soundEffects;
    private LowerHandle LowerHandle;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Ball = GameObject.Find("Ball");
        soundEffects = GetComponent<BallAudio>();
        LowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        //StartCoroutine(LowerHandle.SwitchTo(Ball, 0.2f));
    }

    // Update is called once per frame
    void Update()
    {
        GameObject Ball = GameObject.Find("Ball");
        //StartCoroutine(LowerHandle.SwitchTo(Ball, 0.2f));
    }

    private void FixedUpdate()
    {
        //StartCoroutine(GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(Ball, 0.2f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            Vector3 direction = collision.relativeVelocity;
            rb.AddForce(direction.normalized * 50);
            Debug.Log("Ball Collision"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            soundEffects.PlayGoal();
            //MISSING: Play goal sound
            // Start next Level
        }
        else if (other.gameObject.CompareTag("water"))
        {
            soundEffects.PlayWaterDrop();
            // MISSING: Play Water drop sound
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
