using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Playfield : MonoBehaviour
{
    public static int w = 10;
    public static int h = 18;
    private static bool[,] grid = new bool[w,h];
    private static float StartX = 0f;
    private static float StartZ = 0f;
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

    void updateGrid() {
    // Remove old children from grid

    // Add new children to grid

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

    public static void confirmBlock(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int i;
        int j;
        foreach(Transform child in block.transform) {
            if(child.name!="GameObject") {
            xPosRelative = child.transform.position.x;
            zPosRelative = child.transform.position.z;
            i=(int)(2*xPosRelative);
            j=(int)(2*zPosRelative);
            grid[i,j]=true;
            }
        }
    }

    public static bool isValidPlacement(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int i;
        int j;
        foreach(Transform child in block.transform) {
            if(child.name!="GameObject") {
            xPosRelative = Mathf.Round(child.transform.position.x*2f)*0.5f;
Debug.Log("xPos" + xPosRelative);
            zPosRelative = Mathf.Round(child.transform.position.z*2f)*0.5f;
Debug.Log("yPos" + zPosRelative);
            i=(int)(2*xPosRelative);
            j=(int)(2*zPosRelative);
Debug.Log("i " +i);
Debug.Log("j " +j);
            if(grid[i,j]==true) {
                Debug.Log("False");
                return false;
            }
        }
        }
        return true;
    }
}
