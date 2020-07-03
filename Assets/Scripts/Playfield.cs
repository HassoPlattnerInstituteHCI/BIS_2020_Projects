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
            child.transform.position = new Vector3(Mathf.Round(child.transform.position.x*2f)/2f, 
                                                                Mathf.Round(child.transform.position.y*2f)/2f, 
                                                                Mathf.Round(child.transform.position.z*2f)/2f);
        }
    }

    static void updateTagName(int column, int row, Transform block) {
        block.tag = "inLine"+row;
        block.name = "ArrayCL"+column+row;
        updateGrid();
    }



    static void updateGrid() {
        for(int row=0; row<h; row++) {
            for(int column=0; column<w; column++) {
                grid[column, row] = false;
                if(GameObject.Find("ArrayCL"+column+row)) {
                    grid[column, row] = true;
                } 
            }
        }
    }

    public static void decreaseRowsAbove(int row) {
        GameObject currentBlock;
        for(;row<h-1;row++) {
            for(int column=0; column<w; column++) {
                currentBlock = GameObject.Find("ArrayCL"+column+row);
                if(currentBlock!=null) {
                    currentBlock.transform.position=currentBlock.transform.position + new Vector3(0, 0, (float)-0.5);
                    updateTagName(column, row-1, currentBlock.transform);
                    currentBlock=null;
                }
            }
        }
    }

    public static void checkRows() {
        bool foundOne=false;
        for(int row=0; row<h; row++) {
            if(GameObject.FindGameObjectsWithTag("inLine"+row).Length==w) {
                deleteThisRow(row);
                foundOne=true;
            }
        }
        if(foundOne) {
            checkRows();
        }
    }

    public static void deleteThisRow(int row) {
        GameObject[] theseBlocks;
        theseBlocks = GameObject.FindGameObjectsWithTag("inLine"+row); 
        foreach(GameObject block in theseBlocks) {
            Destroy(block);
        }
        updateGrid();
        decreaseRowsAbove(row+1);
    }

    public static void confirmBlock(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int column;
        int row;
        foreach(Transform child in block.transform) {
            if(child.name!="Rotater") {
                xPosRelative = Mathf.Round(child.transform.position.x*2f)/2f;
                zPosRelative = Mathf.Round(child.transform.position.z*2f)/2f;
                column=(int)(2*xPosRelative);
                row=(int)(2*zPosRelative);
                updateTagName(column, row, child);
            }
        }
    }

    public static bool isValidPlacement(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int column;
        int row;
        int surroundings=0;
        foreach(Transform child in block.transform) {
            if(child.name!="Rotater") {
                //Put the whole function into placeBlock and reuse PosRelative?
                xPosRelative = Mathf.Round(child.transform.position.x*2f)/2f;
//Debug.Log("xPos" + xPosRelative);
                zPosRelative = Mathf.Round(child.transform.position.z*2f)/2f;
//Debug.Log("yPos" + zPosRelative);
                column=(int)(2*xPosRelative);
                row=(int)(2*zPosRelative);
//Debug.Log("column " +column);
//Debug.Log("row " +row);
                
                if(grid[column, row]==true) {
                    Debug.Log("Invalid Placement: Position already occupied");
                    return false;
                }
                //Check for this child if it is on bottom row or below is another block
                if(row==0 || grid[column, row-1]==true) {
                    surroundings++;
                }
            }
        }
        //If none of the blocks are on bottom row or have a block below them, return false
        if(surroundings==0) {
            Debug.Log("Invalid Placement: No surrounding blocks/bottom");
            return false;
        }
        return true;
    }
}
