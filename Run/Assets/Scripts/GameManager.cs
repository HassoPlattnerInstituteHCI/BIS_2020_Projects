using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public Player player;
    public Inspector inspector;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

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
