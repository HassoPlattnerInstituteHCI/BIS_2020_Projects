using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PantoCollider;
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
        //updateGrid(column, row, true);
    }

    static bool checkPosition(int column, int row) {
        if(GameObject.Find("ArrayCL"+column+row)) {
            return true;
        }
        return false;
    }

    static void updateGrid(int column, int row, bool value) {
        grid[column, row] = value;
    }

    public static void decreaseRowsAbove(int i) {
        GameObject currentBlock;
        for(int row=i; row<h-1; row++) {
            for(int column=0; column<w; column++) {
                if(checkPosition(column, row)) {
                    currentBlock = GameObject.Find("ArrayCL"+column+row);
                    currentBlock.transform.position=currentBlock.transform.position + new Vector3(0, 0, (float)-0.5);
                    //updateGrid(column, row, false);
                    updateTagName(column, row-1, currentBlock.transform);
                }
            }
        }
    }

    public static void checkRows() {
        for(int row=h-1; row>=0; row--) {
            if(GameObject.FindGameObjectsWithTag("inLine"+row).Length==w) {
                deleteThisRow(row);
            }
        }
    }

    public static void deleteThisRow(int row) {
        GameObject thisBlock;
        for(int column=0; column<w; column++) {
            thisBlock = GameObject.Find("ArrayCL"+column+row);
            //thisBlock.GetComponent<PantoBoxCollider>().Disable();
            //thisBlock.GetComponent<PantoBoxCollider>().Remove();
            Destroy(thisBlock);
        }
        for(int column=0; column<w; column++) {
            //updateGrid(column, row, false);
        }
        decreaseRowsAbove(row+1);
    }

    public static void confirmBlock(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int column;
        int row;


        foreach (Transform child in block.transform)
        {
            if (child.name != "Rotater")
            {
                xPosRelative = Mathf.Round(child.transform.position.x * 2f) / 2f;
                zPosRelative = Mathf.Round(child.transform.position.z * 2f) / 2f;
                column = (int)(2 * xPosRelative);
                row = (int)(2 * zPosRelative);
                updateTagName(column, row, child);
                //child.GetComponent<PantoBoxCollider>().CreateObstacle();
                //child.GetComponent<PantoBoxCollider>().Enable();
            }
        }
        block.transform.DetachChildren();
        Destroy(GameObject.Find("Rotater"));
        Destroy(block);
        
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
                zPosRelative = Mathf.Round(child.transform.position.z*2f)/2f;
                column=(int)(2*xPosRelative);
                row=(int)(2*zPosRelative);
                if(checkPosition(column, row)) {
                //if(grid[column, row]==true)
                    Debug.Log("Invalid Placement: Position already occupied");
                    return false;
                }
                //Check for this child if it is on bottom row or below is another block
                if(row==0 || checkPosition(column, row-1)) {
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
