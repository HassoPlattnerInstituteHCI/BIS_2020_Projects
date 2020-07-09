using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpeechIO;
using System;
using DualPantoFramework;

namespace dualLayouting {
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
            audioManager.SetCallbacks(OnSelect, OnCreate, OnDelete, OnList, OnShow, OnDone, OnDeleteAll);

            isMoving = false;
            upperHandle = GetComponent<UpperHandle>();
            lowerHandle = GetComponent<LowerHandle>();
            selectedElement = null;

            await StartNextLevel();
            UpdateCommandsElements();
        }
        private void OnDeleteAll()
        {
            GameObject container = GameObject.Find("Elements");

            foreach (Transform child in container.transform)
            {
                GameObject childo = child.gameObject;
                childo.GetComponent<ElementScript>().Delete();
            }
            UpdateCommandsElements();
        }

        private void OnDone()
        {
            StartNextLevel();
        }
        async public Task StartLevelZero()
        {
            Level level = GetComponent<Level>();
            await audioManager.Say("Welcome to Dual Layouting. Let's design a birthday card.");
            await level.PlayIntroduction();

            await Task.WhenAll(new Task[] {
                MoveItToElement(GameObject.Find("Happy Birthday")),
                audioManager.Say("Here is a text reading \"Happy Birthday\".")
            });
            await audioManager.Say("Say \"Select Happy Birthday\". Then try to move it to the center of the page.");
            await audioManager.Say("Say \"Done\" when you are ready for the next step.");

        }
        async public Task StartLevelOne()
        {
            await audioManager.Say("There is a second element on this page.");
            await audioManager.Say("Say \"List Elements\" to get an overview.");
        }
        async public Task StartLevelTwo()
        {
            await audioManager.Say("A pizza doesn't seem to be appropriate on a birthday card, right? To delete it, say \"Delete Pizza\".");
        }
        async public Task StartLevelThree()
        {
            await audioManager.Say("Instead of the pizza, let's insert something fun. How about a clipart of colorful baloons or a bottle of champagne? Say \"Add ballons\" or \"Add champagne\"");
        }
        async public Task StartLevelFour()
        {
            await audioManager.Say("Now position the baloons next to the text. If you do not remember it's position just say \"Show Happy Birthday\".");
        }
        async public Task StartLevelFive()
        {
            await audioManager.Say("Great job! Feel free to continue or start over with a new kick-ass design! Say \"Delete all\" to get a clear page.");
        }

        async private Task StartNextLevel()
        {
            Func<Task>[] startLevels = new Func<Task>[] {StartLevelZero, StartLevelOne, StartLevelTwo, StartLevelThree, StartLevelFour, StartLevelFive};
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
            newElement.GetComponent<ElementScript>().PlayCreateSound();
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

            elementToDelete.GetComponent<ElementScript>().Delete();
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
}