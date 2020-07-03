using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Playfield : MonoBehaviour
{
    public static int w = 10;
    public static int h = 18;
    private static bool[,] grid = new bool[w,h];

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

    static void updateTagName(int column, int row, Transform block) {
        if(block.tag!="Untagged") {
            block.tag = null;
        }
        block.tag = "inLine"+row;
        block.name = "ArrayCL"+column+row;
    }

    public static void decreaseRowsAbove(int row) {
        int column=0;
        GameObject[] currentLineBlocks;
        for(int i=row; i<h; i++) {
            column=0;
            currentLineBlocks = GameObject.FindGameObjectsWithTag("inLine"+row);
            for(int j=0; j<currentLineBlocks.Length; j++) {
                currentLineBlocks[j].transform.position=currentLineBlocks[j].transform.position + new Vector3(0, 0, (float)-0.5);
                grid[column, row] = false;
                grid[column, row-1] = true;
                updateTagName(column, row-1, currentLineBlocks[j].transform);
                column++;
            }
        }
    }

    public static void checkRows() {
        for(int row=0; row<h; row++) {
            if(GameObject.FindGameObjectsWithTag("inLine"+row).Length==w) {
                deleteThisRow(row);
                row=0;
            }
        }
    }

    public static void deleteThisRow(int line) {
        int width=0;
        GameObject[] theseBlocks;
        theseBlocks = GameObject.FindGameObjectsWithTag("inLine"+line); 
        foreach(GameObject block in theseBlocks) {
            Destroy(block);
            grid[width, line] = false;
            width++;
        }
        decreaseRowsAbove(line+1);
    }

    public static void confirmBlock(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int i;
        int j;
        foreach(Transform child in block.transform) {
            if(child.name!="Rotater") {
            xPosRelative = child.transform.position.x;
            zPosRelative = child.transform.position.z;
            i=(int)(2*xPosRelative);
            j=(int)(2*zPosRelative);
            grid[i,j]=true;
            updateTagName(i, j, child);
            }
        }
    }

    public static bool isValidPlacement(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int i;
        int j;
        foreach(Transform child in block.transform) {
            if(child.name!="Rotater") {
            xPosRelative = Mathf.Round(child.transform.position.x*2f)*0.5f;
//Debug.Log("xPos" + xPosRelative);
            zPosRelative = Mathf.Round(child.transform.position.z*2f)*0.5f;
//Debug.Log("yPos" + zPosRelative);
            i=(int)(2*xPosRelative);
            j=(int)(2*zPosRelative);
Debug.Log("i " +i);
Debug.Log("j " +j);
            if(grid[i,j]==true) {
                Debug.Log("False");
                return false;
            }
            //Check here if below is either bottom or another block
            //...
        }
        }
        return true;
    }
}
