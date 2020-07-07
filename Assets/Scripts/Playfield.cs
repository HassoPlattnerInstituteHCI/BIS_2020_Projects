﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework
using static PantoCollider;
public class Playfield : MonoBehaviour
{
    public static int w = 10;
    public static int h = 18;
    private static bool[,] grid = new bool[w,h]; //is currently not used
    static GameObject allRowsParent = GameObject.Find("AllRows");

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //takes the blocks position and aligns all of its children to the .5 by .5 grid
    public static void roundAndPlaceBlock(GameObject block) { 
        //Need to assert that the placement location is valid first!
        foreach(Transform child in block.transform) {
            child.transform.position = new Vector3(Mathf.Round(child.transform.position.x*2f)/2f, 
                                                                Mathf.Round(child.transform.position.y*2f)/2f, 
                                                                Mathf.Round(child.transform.position.z*2f)/2f);
        }

    }

    //sets a blocks name and tag to the column and row it is in in the grid
    static void updateTagName(int column, int row, Transform block) {
        block.tag = ""+column; //need to change tags names to column
        block.name = "ArrayCR"+column+row;
    }

    //checks if a position in the grid is already taken by a block (solution without an actual array, but only with blocks in the scene)
    static bool checkPosition(int column, int row) {
        if(GameObject.Find("ArrayCR"+column+row)) {
            return true;
        }
        return false;
    }

    //For each row, checks if it is full and proceeds accordingly
    public static void deleteFullRows() {
        int maxRow = -1;
        int counter = 0;
        for (int row=0; row<h; row++) { //This first deletes all rows that are full, and counts how many have been deleted
            if (checkRow(row)) {
                deleteThisRow(GameObject.Find("Row"+row));
                if(maxRow<row) {maxRow=row;}
                counter++;
            }
            //Debug.Log("Row: "+row+" maxRow: "+maxRow+" counter: "+counter);
        }
        while(counter>0) { //All rows above the highest fallen row are now decreased as many times as rows have been deleted
            decreaseRowsAbove(maxRow+counter);
            counter--;
        }
    }

    //checks whether or not a row is completely filled with blocks
    public static bool checkRow(int row) {
        if(allRowsParent.transform.GetChild(row).transform.childCount == w) {
            return true;
        }
        return false;
    }

    //deletes one row of blocks
    public static void deleteThisRow(GameObject currentRow) {
        //thisBlock.GetComponent<PantoBoxCollider>().Disable();
        //thisBlock.GetComponent<PantoBoxCollider>().Remove();
        for(int i=9; i>=0; i--) {
            Transform child = currentRow.transform.GetChild(i);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    //for each block in all rows including and above "row", moves the blocks down one row
    public static void decreaseRowsAbove(int row) {
        int column;
        GameObject rowParent;
        for(int currentRow=row; currentRow<h; currentRow++) { //do this for every row above and including the current one
        rowParent = allRowsParent.transform.GetChild(currentRow).gameObject; //get the parent object of the current row
            foreach (Transform child in rowParent.transform) {
                child.transform.position=child.transform.position + new Vector3(0, 0, (float)-0.5); //set the childs z-position amount times .5 lower
                column = int.Parse(child.tag); //gets the child column tag
                updateTagName(column, currentRow-1, child.transform); 
            }
            updateRowParents(currentRow-1, rowParent); //After all children have been renamed and moved, change their parent to be the new row
        }
    }

    //is called when after a row is decreased. Changes all blocks in the old row to be children of their new row
    static void updateRowParents(int newRow, GameObject oldRow) {
        Transform child;
        int children = oldRow.transform.childCount;
        for(int block=0; block<children; block++) {
            child=oldRow.transform.GetChild(0);
            child.parent = null;
            child.parent = allRowsParent.transform.GetChild(newRow);
        }
    }

    //is called when the player wants to place the block in the current position. For each child, except the rotater (which is deleted), the coordinates are confirmed
    //and the block gets the appropiate name and tag
    public static void confirmBlock(GameObject block) {
        float xPosRelative;
        float zPosRelative;
        int column;
        int row;
        GameObject parentRow;
        GameObject rotater = block.transform.GetChild(0).gameObject;
        rotater.transform.parent = null;
        Destroy(rotater); //Destroys Rotater
        for(int i=block.transform.childCount-1; i>=0; i--) {
            xPosRelative = Mathf.Round(block.transform.GetChild(i).transform.position.x * 2f) / 2f;
            zPosRelative = Mathf.Round(block.transform.GetChild(i).transform.position.z * 2f) / 2f;
            column = (int)(2 * xPosRelative);
            row = (int)(2 * zPosRelative);
            updateTagName(column, row, block.transform.GetChild(i));
            parentRow = GameObject.Find("Row"+row);
            Debug.Log(block.transform.GetChild(i).transform);
            block.transform.GetChild(i).transform.SetParent(parentRow.transform);
            //block.transform.GetChild(i).GetComponent<PantoBoxCollider>().CreateObstacle();
            //block.transform.GetChild(i).GetComponent<PantoBoxCollider>().Enable();
        }
        Destroy(block);
    }

    //is called when the player attempts to place a block. Rounds the coordinates of all children (except rotater) and checks whether or not there already is a block in 
    //that position in the grid. Additionally, checks if at least one other block has a neighbour below/is in the bottom row 
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
