using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    protected virtual bool WinConditions()
    { 
        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ball"))
        {
            if (WinConditions())
            {
                Debug.Log("Ball im Goal!");
                StartCoroutine(SoundAndNextLevel(1));
            }
        }
    }

    IEnumerator SoundAndNextLevel(int delay)
    {
        //ölaksjd föoqiwj;
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }
    private void LoadNextLevel()
    {
        //SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % (SceneManager.sceneCountInBuildSettings));
    }
}
