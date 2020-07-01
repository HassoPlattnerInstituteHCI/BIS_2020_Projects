using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
   
    private GameObject selectedElement;
    private UpperHandle upperHandle;
    private LowerHandle lowerHandle;

    // Start is called before the first frame update
    void Start()
    {
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        selectedElement = null;
        Level level = GetComponent<Level>();
        

        level.PlayIntroduction();

        SelectElement(GameObject.Find("Sun"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectElement(GameObject element)
    {
        selectedElement = element;
        //ElementScript script = selectedElement.GetComponent<ElementScript>(); 
        upperHandle.SwitchTo(selectedElement, 1.0f);
    }
}
