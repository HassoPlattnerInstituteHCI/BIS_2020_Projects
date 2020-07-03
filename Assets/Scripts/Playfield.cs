using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    // The Grid itself
    public static int w = 10;
    public static int h = 18;
    public static bool[,] grid = new bool[w,h];
    private float newValue;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void roundAndPlaceBlock(GameObject block) { //takes the blocks position and rounds the coordinates 
        //Need to assert that the placement location is valid first!
        foreach(Transform child in block.transform) {
            child.transform.position = new Vector3(Mathf.Round(child.transform.position.x*2f)*0.5f, 
                                                                Mathf.Round(child.transform.position.y*2f)*0.5f, 
                                                                Mathf.Round(child.transform.position.z*2f)*0.5f);
        }

        
    }

    public static bool insideBorder(Vector3 pos) {
        return true;
    }

    public static void deleteRow(int y) {

    }

    public static void decreaseRow(int y) {

    }

    public static void decreaseRowsAbove(int y) {

    }

    public static bool isRowFull(int y) {
        return true;
    }

    public static void deleteFullRows() {

    }

    public static bool isValidPlacement() {
        bool value = false;
        
        return value;
    }
}
