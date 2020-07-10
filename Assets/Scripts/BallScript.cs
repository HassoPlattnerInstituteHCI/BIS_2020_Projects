using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PantoGolf
{
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
            Vector3 goal_dir = GameObject.Find("Goal").transform.position - transform.position;
            //LowerHandle.transform.LookAt(goal_dir);
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

        private async void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("goal"))
            {
                Debug.Log("The ball hit the goal!");
                soundEffects.PlayGoal();
                rb.velocity = Vector3.zero;
                // Start next level
                StartCoroutine(levelComplete());
                // Start next Level
            }
            else if (other.gameObject.CompareTag("water"))
            {
                Debug.Log("Ball fell in water!");
                rb.velocity = Vector3.zero;
                soundEffects.PlayWaterDrop();
                StartCoroutine(RestartLevel());
            }
        }


        IEnumerator RestartLevel()
        {
            yield return new WaitForSeconds(2);
            GameObject.Find("Panto").GetComponent<GameManager>().RestartLevel();
        }

        IEnumerator levelComplete()
        {
            yield return new WaitForSeconds(2);
            GameObject.Find("Panto").GetComponent<GameManager>().LevelComplete();
        }
    }
}
