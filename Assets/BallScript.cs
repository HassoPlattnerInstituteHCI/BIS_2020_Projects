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
        //Debug.Log("Ball Collision.");
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Debug.Log("Ball hit wall/obstacle.");
            soundEffects.PlayObstacle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            Debug.Log("The ball hit the goal!");
            soundEffects.PlayGoal();
            rb.velocity = Vector3.zero;
            GameObject.Find("Panto").GetComponent<GameManager>().LevelComplete();
            // Start next Level
        }
        else if (other.gameObject.CompareTag("water"))
        {
            Debug.Log("Ball fell in water!");
            rb.velocity = Vector3.zero;
            soundEffects.PlayWaterDrop();
            RestartLevel();
        }
    }


    public void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
