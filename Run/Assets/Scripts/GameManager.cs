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
    public Inspector inspector;
    public GameObject[] Stages;
    AudioSource audioSource;

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
        player.freeze();
        speechOut.Speak("Welcome to Super Mario! You are in a twodimensional world. You can move around with the me handle! When you move it up to jump, it will make the following sound");

        yield return new WaitForSeconds(10);
        player.PlaySound("Jump");
        audioSource.Play();
        yield return new WaitForSeconds(1);

        speechOut.Speak("Ok, Great! Move left or right to move around in the World. Also by moving the it handle you can explore the environment");
        speechOut.Speak("Listen closely");

        yield return new WaitForSeconds(10);
        player.PlaySound("Ground");
        audioSource.Play();
        yield return new WaitForSeconds(1);

        speechOut.Speak("When you collide with an object it will create a sound. In this case it was an obstacle or the ground!");
        speechOut.Speak("Now Try to reach the end of the level on the right.");
        yield return new WaitForSeconds(10);
        Debug.Log("DONE");
        player.unfreeze();
    }

    IEnumerator tutorialLevel2()
    {
        player.freeze();
        speechOut.Speak("There is an enemy around you! Listen");

        yield return new WaitForSeconds(5);
        player.PlaySound("Damaged");
        audioSource.Play();
        yield return new WaitForSeconds(1);

        speechOut.Speak("Remember: You can distinguish Objects by their sound. It this case you collided with an enemy!");
        speechOut.Speak("Try to reach the end of the level. You can kill the enemy by jumping on him.");
        yield return new WaitForSeconds(10);
        Debug.Log("DONE");
        player.unfreeze();
    }

    IEnumerator tutorialLevel3()
    {
        player.freeze();
        speechOut.Speak("“There are 6 coins in this level. You will hear a sound when you collect them.");
        speechOut.Speak("Try to collect some of them while completing the level!.");
        yield return new WaitForSeconds(10);
        Debug.Log("DONE");
        player.unfreeze();
    }

    IEnumerator tutorialLevel4()
    {
        player.freeze();
        speechOut.Speak("“Somewhere in this level is a item-box. Collect the item by jumping against it! You will hear a sound when you do.");
        speechOut.Speak("Now complete the level by reaching the flag on the right!.");
        yield return new WaitForSeconds(12);
        Debug.Log("DONE");
        player.unfreeze();
    }

    IEnumerator tutorialLevel5()
    {
        player.freeze();
        speechOut.Speak("“Be aware of holes in the ground!");
        yield return new WaitForSeconds(1);
        player.unfreeze();
    }



    void update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();

    }

    // Start is called before the first frame update

    async public void NextStage()
    {
        //Stage Change
        if (stageIndex < Stages.Length - 1)
        {

            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            if (stageIndex == 1)
            {
                StartCoroutine(tutorialLevel2());
            }
            if (stageIndex == 2)
            {
                StartCoroutine(tutorialLevel3());
            }
            if (stageIndex == 3)
            {
                StartCoroutine(tutorialLevel4());
            }
            if (stageIndex == 4)
            {
                StartCoroutine(tutorialLevel5());
            }
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

    async public void PlayerReposition()
    {
        player.transform.position = new Vector3(-80, -30, 0);
        player.Reposition();
        player.VelocityZero();
    }

    public void InspectorReposition()
    {
        inspector.transform.position = new Vector3(-80, -30, 0);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }


}
