using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System;

public class AppManager : MonoBehaviour
{
   
    private GameObject selectedElement;
    private UpperHandle upperHandle;
    private LowerHandle lowerHandle;
    private bool isMoving;

    private AudioManager audioManager;

    public GameObject elementPrefab;

    private int currLevel;

    // Start is called before the first frame update
    async void Start()
    {
        currLevel = 0;
        audioManager = GetComponent<AudioManager>();
        audioManager.SetCallbacks(OnSelect, OnCreate, OnDelete, OnList, OnShow, OnDone);

        isMoving = false;
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        selectedElement = null;

        await StartNextLevel();
        UpdateCommandsElements();
    }

    private void OnDone()
    {
        StartNextLevel();
    }
    async public Task StartLevelZero()
    {
        Level level = GetComponent<Level>();
        await level.PlayIntroduction();

        await Task.WhenAll(new Task[] {
            MoveItToElement(GameObject.Find("Sun")),
            audioManager.Say("Here is a graphic of a sun.")
        });
        await audioManager.Say("Say \"Select sun\". Then try to move it to the upper right corner.");
        await audioManager.Say("Say \"Done\" to continue.");
    
    }
    async public Task StartLevelOne()
    {
        await audioManager.Say("There is a second element on this page.");
        await audioManager.Say("Say \"List Elements\" to get an overview.");
    }
    async public Task StartLevelTwo()
    {
        await audioManager.Say("You can delete elements. Say \"Delete Otter\".");
    }
    async public Task StartLevelThree()
    {
        await audioManager.Say("Instead of the otter, let's add a picture of clouds. Say \"Create clouds\".");
    }
    async private Task StartNextLevel()
    {
        Func<Task>[] startLevels = new Func<Task>[] {StartLevelZero, StartLevelOne, StartLevelTwo, StartLevelThree};
        await startLevels[currLevel]();
        currLevel++;
    }
    private void OnSelect(string name)
    {
        GameObject toSelect = GameObject.Find(name);
        if (toSelect == null)
        {
            return;
        }
        SelectElement(toSelect);
    }
    private void OnCreate(string addedName)
    {
        GameObject newElement = Instantiate(elementPrefab);
        newElement.transform.parent = GameObject.Find("Canvas/Elements").transform;
        newElement.name = addedName;
        SelectElement(newElement);
        UpdateCommandsElements();
    }
    
    async private void OnShow(string element)
    {    
        GameObject elementToShow = GameObject.Find(element);
        ShowElement(elementToShow);
    }

    async public Task ShowElement(GameObject element)
    {
        Task[] tasks = new Task[] {
            audioManager.Say(element.name), 
            MoveItToElement(element)
        };
    
        await Task.WhenAll(tasks);
    }

    async private void OnList()
    {
        await audioManager.Say($"There are {GetElements().Length} elements.");
        foreach (GameObject element in GetElements())
        {
            await ShowElement(element);
        }  
    }

    private void OnDelete(string elementName)
    {
        GameObject elementToDelete = GameObject.Find(elementName);
        
        // should never be entered        
        if (elementToDelete == null)
        {
            return;
        }

        if (selectedElement == elementToDelete){
            selectedElement = null;
        }

        Destroy(elementToDelete);
        UpdateCommandsElements();
    }

    private GameObject[] GetElements()
    {
        return GameObject.FindGameObjectsWithTag("Element");
    }
    private void UpdateCommandsElements()
    {
        GameObject[] elements = GetElements();
        audioManager.UpdateCommands(elements);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelectedElementPosition();
    }

    private void UpdateSelectedElementPosition()
    {
        if (isMoving == true || selectedElement == null)
        {
            return;
        }
        selectedElement.transform.position = upperHandle.GetPosition();
    }

    async public Task SelectElement(GameObject element)
    {
        isMoving = true;
        selectedElement = element; 
        await upperHandle.MoveToPosition(selectedElement.transform.position, 0.2f);
        isMoving = false;
    }

    async public Task MoveItToElement(GameObject element)
    { 
        await lowerHandle.MoveToPosition(element.transform.position, 0.2f, false);
    }

}
