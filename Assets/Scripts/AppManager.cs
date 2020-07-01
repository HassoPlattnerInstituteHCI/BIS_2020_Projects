using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AppManager : MonoBehaviour
{
   
    private GameObject selectedElement;
    private UpperHandle upperHandle;
    private LowerHandle lowerHandle;
    private bool isMoving;

    private AudioManager audioManager;

    // Start is called before the first frame update
    async void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioManager.SetCallbacks(OnSelect);

        isMoving = false;
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        selectedElement = null;
        Level level = GetComponent<Level>();
        

        await level.PlayIntroduction();
        await SelectForTutorial();
        UpdateCommandsElements();
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
    private void UpdateCommandsElements()
    {
        GameObject[] elements = GameObject.FindGameObjectsWithTag("Element");
        audioManager.UpdateCommands(elements);
    }
    async public Task SelectForTutorial()
    {
        Task[] tasks = new Task[2];
        tasks[0] = audioManager.IntroduceFirstElement();
        tasks[1] = SelectElement(GameObject.Find("Sun"));
        
        await Task.WhenAll(tasks);
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
}
