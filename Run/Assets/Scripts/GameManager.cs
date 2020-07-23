using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpeechIO;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public Player player;
    AudioSource audioSource;
    public Inspector inspector;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    SpeechOut speechOut;

    void Start()
    {
        speechOut = new SpeechOut();
        audioSource = player.GetComponent<AudioSource>();
        StartCoroutine(tutorialLevel1());

    }

    IEnumerator tutorialLevel1()
    {
        speechOut.Speak("Welcome to Super Mario! You are in a twodimensional world. You can move around with the me handle! When you move it up to jump, it will make the following sound");

        yield return new WaitForSeconds(12);
        player.PlaySound("Jump");
        audioSource.Play();
        yield return new WaitForSeconds(2);

        speechOut.Speak("Ok, Great! Move left or right to move around in the World. Also by moving the it handle you can explore the environment");
        speechOut.Speak("Listen closely");

        yield return new WaitForSeconds(12);
        player.PlaySound("Coin");
        audioSource.Play();
        yield return new WaitForSeconds(2);

        speechOut.Speak("When you collide with an object it will create a sound. In this case it was a coin!");
        speechOut.Speak("Now Try to reach the end of the level on the right.");
    }

    IEnumerator tutorialLevel2()
    {
        speechOut.Speak("There is an enemy around you! Listen");

        yield return new WaitForSeconds(5);
        player.PlaySound("Damaged");
        audioSource.Play();
        yield return new WaitForSeconds(2);

        speechOut.Speak("Remember: You can distinguish Objects by their sound. It this case you collided with an enemy!");
        speechOut.Speak("Try to reach the end of the level. You can kill the enemy by jumping on him.");
    }

    IEnumerator tutorialLevel3()
    {
        speechOut.Speak("Somewhere in this level is a item-box. Collect the item by jumping against it! You will hear a sound when you do.");
        speechOut.Speak("Now complete the level by reaching the flag on the right!.");
        yield return new WaitForSeconds(0);
    }

    void update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    // Start is called before the first frame update

    public void NextStage()
    {

        //Stage Change
        if (stageIndex < Stages.Length - 1)
        {        
            if(stageIndex == 1)
            {
                StartCoroutine(tutorialLevel2());
            }
            if (stageIndex == 2)
            {
                StartCoroutine(tutorialLevel3());
            }

            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            InspectorReposition();

            UIStage.text = "Stage " + (stageIndex + 1);
        } else //Game Clear
        {
            Time.timeScale = 0;
            Debug.Log("Game Clear");
            RestartButton.SetActive(true);
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "Game Clear";
            RestartButton.SetActive(true);
        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }


    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        } else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            // Player Die Effect
            player.OnDie();
            RestartButton.SetActive(true);


        }
    }

    public void HealthUp()
    {
        if (health < 3)
        {
            health++;
            UIhealth[health-1].color = new Color(1, 1, 1, 1);
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Player Reposition
            if (health > 1)
            {
                PlayerReposition();
            }

            //Health Down
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-80, -30, 0);
        player.Reposition();
        player.VelocityZero();
    }

    void InspectorReposition()
    {
        inspector.transform.position = new Vector3(-80, -30, 0);
        inspector.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
