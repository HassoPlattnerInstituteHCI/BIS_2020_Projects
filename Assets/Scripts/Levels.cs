using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
using SpeechIO;
using System;

public class Levels : MonoBehaviour
{
    SpeechOut speechOut;
    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    GameManager gm;
    public GameObject player;
    public GameObject enemy;
    public GameObject[] obstacles;
    public GameObject powerUp;

    // Start is called before the first frame update
    void Start()
    {
        speechOut = new SpeechOut();
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        gm = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public Task PlayIntroduction(int level,SpeechIn speechIn)
    {

        //await upperHandle.MoveToPosition(gm.playerSpawn.position + new Vector3(2, 0, 2), 0.3f, false);
        GameObject playerHelper = Instantiate(new GameObject(), gm.playerSpawn.position, gm.playerSpawn.rotation);
        GameObject enemyHelper = Instantiate(new GameObject(), gm.enemySpawn.position, gm.enemySpawn.rotation);
        switch (level)
        {
            case 0:
                Level lvl = GetComponent<Level>();
                await lvl.PlayIntroduction();
                // TODO: 2. Explain enemy and player with weapons by wiggling and playing shooting sound                                                LINO HELLIGE
                await speechOut.Speak("Oh no there is an enemy");
                lowerHandle = GetComponent<LowerHandle>();
                await lowerHandle.SwitchTo(enemyHelper, 0.5f);

                await speechOut.Speak("he is trying to kill you");

                await upperHandle.SwitchTo(playerHelper, 0.2f);

                await speechOut.Speak("so aim at him");
                //wiggling lef and right to show how to shoot                                                                                           LINO HELLIGE
                await RotateX(playerHelper);

                break;
            //Level 2                                                                                                                                      OLIVER SCHULZ
            case 1:
                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await speechOut.Speak("Oh no the enemy spotted you, move to another position...");
                await MoveX(playerHelper);
                await speechOut.Speak("...and kill the enemy...");
                await lowerHandle.SwitchTo(enemy, 0.2f);
                await speechOut.Speak("by aiming at him.");
                //upperHandle.Free();
                await RotateX(playerHelper);
                break;
            //Level 3                                                                                                                                      OLIVER SCHULZ
            case 2:
                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await lowerHandle.SwitchTo(playerHelper, 0.2f); //player shouldn't know where enemy is
                await speechOut.Speak("Oh no the enemy escaped, move around...");
                await MoveX(playerHelper);
                await speechOut.Speak("...and watch out...");
                upperHandle.Free();
                await RotateX(playerHelper);
                await speechOut.Speak("for the enemy to kill it.");
                break;
            //Level 4
            case 3:

                
                foreach(GameObject obst in obstacles)
                {
                    obst.SetActive(true);
                    await IntroduceObject(obst.GetComponent<ObjectOfInterest>());
                }
                powerUp.SetActive(true);
                await IntroduceObject(powerUp.GetComponent<ObjectOfInterest>());

                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await speechOut.Speak("The enemy dropped an item behind a wall, move around...");
                await MoveX(playerHelper);
                await speechOut.Speak("...and watch out...");
                //upperHandle.Free();
                await RotateX(playerHelper);
                await speechOut.Speak("to find the wall and the item. I will tell you when you look at them.");
                break;

            default: break;
        }
        upperHandle.Free();

        await speechOut.Speak("The gun will automaticly shoot for you");
        await speechOut.Speak("Feel for yourself. Say yes or done when you're ready.");
        //string response = await speechIn.Listen(commands);
        await speechIn.Listen(new Dictionary<string, KeyCode>() { { "yes", KeyCode.Y }, { "done", KeyCode.D } });
        Destroy(playerHelper);
        Destroy(enemyHelper);
    }

    async private Task IntroduceObject(ObjectOfInterest objectOfInterest)
    {
        Task[] tasks = new Task[2];
        tasks[0] = speechOut.Speak(objectOfInterest.description);

        PantoHandle pantoHandle = objectOfInterest.isOnUpper
            ? (PantoHandle) GetComponent<UpperHandle>()
            : (PantoHandle) GetComponent<LowerHandle>();

        if (objectOfInterest.traceShape)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in objectOfInterest.transform)
            {
                children.Add(child.gameObject);
            }
            children.Sort((GameObject g1, GameObject g2) => g1.name.CompareTo(g2.name));
            tasks[1] = pantoHandle.TraceObjectByPoints(children, 0.2f);
        }
        else
        {
            tasks[1] = pantoHandle.SwitchTo(objectOfInterest.gameObject, 0.2f);
        }
        await Task.WhenAll(tasks);
        await Task.Delay(500);
    }

    async Task Move(GameObject obj, float x, float y, float z, int n) //Move to position in n steps
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 v = new Vector3((float)x / n, y, (float)z / n);
            obj.transform.position = obj.transform.position + v;
            await Task.Delay(10);
        }
    }

    async Task MoveX(GameObject obj)
    {
        await Move(obj, 0, 0, 1, 10);
        await Move(obj, 0, 0, -1, 10);
        await Move(obj, 1, 0, 0, 10);
        await Move(obj, -1, 0, 0, 10);
        await Move(obj, 0, 0, -1, 10);
        await Move(obj, 0, 0, 1, 10);
        await Move(obj, -1, 0, -0, 10);
        await Move(obj, 1, 0, -0, 10);
    }

    async Task RotateX(GameObject obj)
    {
        for (int i = 0; i < 30; i++)
        {
            obj.transform.Rotate(0, -1f, 0);
            await Task.Delay(10);
        }
        for (int i = 0; i < 60; i++)
        {
            obj.transform.Rotate(0, 1f, 0);
            await Task.Delay(10);
        }
        for (int i = 0; i < 30; i++)
        {
            obj.transform.Rotate(0, -1f, 0);
            await Task.Delay(10);
        }
    }

    /*async Task MoveX() //Move in X
    {
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0.5f, 0, 0), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(-0.5f, 0, 0), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(-0.5f, 0, 0), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0.5f, 0, 0), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, 0.5f), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, -0.5f), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, -0.5f), 0.1f, false);
        await Task.Delay(200);
        await upperHandle.MoveToPosition(upperHandle.GetPosition() + new Vector3(0, 0, 0.5f), 0.1f, false);
    }

    async Task RotateX()//Rotate in X
    {
        for (int i = 0; i <= 60; i += 10)
        {
            float r = upperHandle.transform.eulerAngles.y + (float)i;
            upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
            await Task.Delay(100);
        }
        for (int i = 60; i >= -60; i -= 10)
        {
            float r = upperHandle.transform.eulerAngles.y + (float)i;
            upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
            await Task.Delay(100);
        }
        for (int i = -60; i <= 0; i += 10)
        {
            float r = upperHandle.transform.eulerAngles.y + (float)i;
            upperHandle.SetPositions(upperHandle.GetPosition(), r, null);
            await Task.Delay(100);
        }
    }*/
}
