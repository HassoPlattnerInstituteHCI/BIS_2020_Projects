using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using System.Threading.Tasks;
using SpeechIO;
using System;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    SpeechOut speechOut;
    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    GameManager gm;
    public GameObject player;
    public GameObject enemy;
    public GameObject[] obstacles;
    public GameObject[] powerUps;
    Shooting shooting;
    public int gun = 1;
    Scene currentScene;

    // Start is called before the first frame update
    void Start()
    {
        speechOut = new SpeechOut();
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        gm = GetComponent<GameManager>();
        shooting = gm.player.GetComponent<Shooting>();
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async public Task PlayIntroduction(int level,SpeechIn speechIn)
    {

        //await upperHandle.MoveToPosition(gm.playerSpawn.position + new Vector3(2, 0, 2), 0.3f, false);
        GameObject playerHelper = new GameObject();
        playerHelper.transform.position = gm.playerSpawn.position;
        playerHelper.transform.rotation = gm.playerSpawn.rotation;
        GameObject enemyHelper = new GameObject();
        enemyHelper.transform.position = gm.enemySpawn.position;
        enemyHelper.transform.rotation = gm.enemySpawn.rotation;
        switch (level)
        {
            case 0:
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
                await RotateX(playerHelper);
                break;
            //Level 3                                                                                                                                      OLIVER SCHULZ
            case 2:
                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await speechOut.Speak("Oh no the enemy escaped, move around...");
                await MoveX(playerHelper);
                await speechOut.Speak("...and watch out...");
                await RotateX(playerHelper);
                await speechOut.Speak("for the enemy to kill it.");
                break;
            //Level 4
            case 3:
                shooting.spotted = false;

                foreach (GameObject obst in obstacles)
                {
                    obst.SetActive(true);
                    await IntroduceObject(obst.GetComponent<ObjectOfInterest>());
                }
                foreach (GameObject powerUp in powerUps)
                {
                    powerUp.SetActive(true);
                    await IntroduceObject(powerUp.GetComponent<ObjectOfInterest>());
                }
                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await speechOut.Speak("The enemy dropped an item behind a wall, move around...");
                await MoveX(playerHelper);
                await speechOut.Speak("...and watch out...");
                await RotateX(playerHelper);
                await speechOut.Speak("for the wall and the item. I will tell you when you found it.");
                break;
            //level 5
            case 4:
                shooting.spotted = false;

                foreach (GameObject obst in obstacles)
                {
                    obst.SetActive(true);
                    await IntroduceObject(obst.GetComponent<ObjectOfInterest>());
                }

                foreach (GameObject powerUp in powerUps)
                {
                    powerUp.SetActive(true);
                    await IntroduceObject(powerUp.GetComponent<ObjectOfInterest>());
                }

                await upperHandle.SwitchTo(playerHelper, 0.2f);
                await speechOut.Speak("This time you can switch your weapon, by saying weapon one (one is your MG), weapon two (two is your Sniper) or weapon three (three is your pumpgun)");
                break;

            default: break;
        }

        await speechOut.Speak("The gun will automaticly shoot for you");

        if (level != 0)
        {
            upperHandle.Free();
        }
        else
        {
            await upperHandle.MoveToPosition(playerHelper.transform.position, 0.2f, false);
            upperHandle.FreeRotation();
        }

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

    async public void GunListener(SpeechIn speechIn)
    {
        Dictionary<string, KeyCode> command = GetComponent<GameManager>().commands;
        string input = await speechIn.Listen(command);
        switch (input)
        {
            case "weapon one": //MG/normal
                shooting.fireSpreadAngle = 5f;
                shooting.damage = 4;
                shooting.maxRayDistance = 10f;
                shooting.cooldown = 0.1f;
                shooting.startWidth = 0.5f;
                shooting.endWidth = 0.5f;
                gun = 1;
                await speechOut.Speak("weapon one");
                break;
            case "weapon two": //Sniper
                shooting.fireSpreadAngle = 1f;
                shooting.damage = 40;
                shooting.maxRayDistance = 20f;
                shooting.cooldown = 1f;
                shooting.startWidth = 0.1f;
                shooting.endWidth = 0.1f;
                gun = 2;
                await speechOut.Speak("weapon two");
                break;
            case "weapon three": //pump
                shooting.fireSpreadAngle = 20f;
                shooting.damage = 70;
                shooting.maxRayDistance = 4f;
                shooting.cooldown = 1f;
                shooting.startWidth = 0.1f;
                shooting.endWidth = 2f;
                gun = 3;
                await speechOut.Speak("weapon three");
                break;
            default: break;
        }
        //Put in switching sound
        GunListener(speechIn);
    }
}
