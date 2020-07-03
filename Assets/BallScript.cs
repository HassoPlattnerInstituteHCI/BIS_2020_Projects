using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    private Rigidbody rb;
    private GameObject Ball;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Ball = GameObject.Find("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        GameObject Ball = GameObject.Find("Ball");
        StartCoroutine(GameObject.Find("Panto")
                       .GetComponent<LowerHandle>()
                       .SwitchTo(Ball, 0.2f));
    }

    private void FixedUpdate()
    {
        //StartCoroutine(GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(Ball, 0.2f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goal"))
        {
            //MISSING: Play goal sound
            // Start next Level
        }
        else if (other.gameObject.CompareTag("water"))
        {
            // MISSING: Play Water drop sound
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
