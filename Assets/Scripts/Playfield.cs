using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;

namespace Tetris {
public class Playfield : MonoBehaviour
{
    UpperHandle upperHandle;
    LowerHandle lowerHandle;
    public bool shouldFreeHandle;

    public static int offsetX = 2;
    public static int offsetZ = 14;

    static SpeechOut speechOut;
    public static int w = 10;
    public static int h = 18;
    public static GameObject allRowsParent; //Parent Object of all the Row-Objects that are used to track blocks positions
    GameManager Manager;
    static GameObject player; //can it be static?
    static GameObject upperPosition;

    public AudioClip RowClear, LevelClear, LevelFail, BlockPlace, BlockRotate, BlockMove, MovementBlocked, PlacementAbort, ConfirmPlacement;
    public AudioSource audioSource;
    public float volume = 0.5f;
    Vector3[] oldBlockPos = new Vector3[4];

        //Info on blocks in general: Every block is named after its exact position, e.g. "ArrayCR10" would be the block in the bottom row in the first column.
        //Additionally, every block is set as a child of a "RowX" object, each of which (in theory) can only have ten children: One for every column. 
        //Every block also has a tag that indicates its column, this is used when renaming them after decreasing their height/changing which row they are in.
    void Awake()
    {
        allRowsParent = GameObject.Find("AllRows");
        Manager = GameObject.Find("Panto").GetComponent<GameManager>();
    }
        // Start is called before the first frame update
        void Start()
    {
        player = GameObject.Find("Player");
        speechOut = new SpeechOut();
        upperHandle = GetComponent<UpperHandle>();
        lowerHandle = GetComponent<LowerHandle>();
        upperPosition = GameObject.Find("MeHandlePrefab(Clone)").transform.GetChild(0).gameObject;
    }

    public void alignLive(GameObject block, int blockID) {
        //More functions: Check if still in bounds. If newPos!=oldPos play sound
        Vector3[] newBlockPos = new Vector3[4];
        newBlockPos[0] = alignBlockPos(block.transform.GetChild(0).gameObject, blockID); 
        newBlockPos[1] = alignBlockPos(block.transform.GetChild(1).gameObject, blockID); 
        newBlockPos[2] = alignBlockPos(block.transform.GetChild(2).gameObject, blockID); 
        newBlockPos[3] = alignBlockPos(block.transform.GetChild(3).gameObject, blockID); 
        bool notOld = notOldPos(newBlockPos);
        bool notInter = notIntersecting(newBlockPos);
        if(notOld && notInter) {
            oldBlockPos[0] = newBlockPos[0];
            oldBlockPos[1] = newBlockPos[1];
            oldBlockPos[2] = newBlockPos[2];
            oldBlockPos[3] = newBlockPos[3];
            audioSource.PlayOneShot(BlockMove, volume);
            switch(blockID) {
                case 0:
                case 3:
                    player.transform.position = new Vector3(Mathf.Round(upperPosition.transform.position.x*2f)/2f, Mathf.Round(upperPosition.transform.position.y*2f)/2f, Mathf.Round(upperPosition.transform.position.z*2f)/2f);
                    break;
                case 1:
                case 2:
                case 4:
                case 5:
                case 6:
                    player.transform.position = new Vector3(Mathf.Round(upperPosition.transform.position.x*2f)/2f, Mathf.Round(upperPosition.transform.position.y*2f)/2f, (Mathf.Round((upperPosition.transform.position.z+0.25f)*2f)/2f));
                    break;
        }
    }
    else {
        player.transform.position = oldBlockPos[0];
        if(!notInter) { 
            audioSource.PlayOneShot(MovementBlocked, volume);
        }
    }
}
    //takes the childs position and align it to the grid
    public Vector3 alignBlockPos(GameObject block, int version) { 
        if(version==0 || version==3) {
            return new Vector3(Mathf.Round(upperPosition.transform.position.x*2f)/2f, Mathf.Round(upperPosition.transform.position.y*2f)/2f, Mathf.Round(upperPosition.transform.position.z*2f)/2f);
        } else {return new Vector3(Mathf.Round(upperPosition.transform.position.x*2f)/2f, Mathf.Round(upperPosition.transform.position.y*2f)/2f, (Mathf.Round((upperPosition.transform.position.z+0.25f)*2f)/2f));}
    }

    //for every Vector3 in newPosition, check if it is in fact the old Position
    public bool notOldPos(Vector3[] newPosition) {
        for(int i=0; i<4; i++) {
            if(oldBlockPos[i]==newPosition[i]) {
                return false;
            }
        }
        return true;
    }

    //for every Vector3 of the new Position, check if that is already occupied or out of bounds
    public bool notIntersecting(Vector3[] position) {
        for(int i=0; i<4; i++) {
            float xPosRelative = (Mathf.Round(position[i].x*2f)/2f);
            float zPosRelative = (Mathf.Round(position[i].z*2f)/2f);
            int column=(int)(2*(xPosRelative+offsetX));
            int row=(int)(2*(zPosRelative+offsetZ));
            if(checkPosition(column, row)) {
                Debug.Log("Invalid Placement: Position already occupied");
                return false;
            }
        }
        return true;
    }

    public int rotateBlock(GameObject block, int blockID, int rotateAmount) {
        GameObject child1 = block.transform.GetChild(0).gameObject;
        GameObject child2 = block.transform.GetChild(1).gameObject;
        GameObject child3 = block.transform.GetChild(2).gameObject;
        GameObject child4 = block.transform.GetChild(3).gameObject;
        audioSource.PlayOneShot(BlockRotate, volume);
        switch (blockID) {
            case 0:
                if(rotateAmount==0 || rotateAmount==2) {
                    child1.transform.position = new Vector3(child1.transform.position.x - 0.5f, child1.transform.position.y, child1.transform.position.z + 1f);
                    child2.transform.position = new Vector3(child2.transform.position.x, child2.transform.position.y, child2.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z);
                    child4.transform.position = new Vector3(child4.transform.position.x + 1f, child4.transform.position.y, child4.transform.position.z - 0.5f);
                } else if(rotateAmount == 1 || rotateAmount == 3) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 0.5f, child1.transform.position.y, child1.transform.position.z - 1f);
                    child2.transform.position = new Vector3(child2.transform.position.x, child2.transform.position.y, child2.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z);
                    child4.transform.position = new Vector3(child4.transform.position.x - 1f, child4.transform.position.y, child4.transform.position.z + 0.5f);
                }
                break;
            case 1:
                if(rotateAmount==0) {
                    child1.transform.position = new Vector3(child1.transform.position.x - 0.5f, child1.transform.position.y, child1.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 1f, child4.transform.position.y, child4.transform.position.z);
                } else if (rotateAmount==1) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 0.5f, child1.transform.position.y, child1.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x, child4.transform.position.y, child4.transform.position.z + 1f);
                } else if (rotateAmount==2) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 0.5f, child1.transform.position.y, child1.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 1f, child4.transform.position.y, child4.transform.position.z);
                } else if (rotateAmount==3) {
                    child1.transform.position = new Vector3(child1.transform.position.x - 0.5f, child1.transform.position.y, child1.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x, child4.transform.position.y, child4.transform.position.z - 1f);
                }
                break;
            case 2:
                if(rotateAmount==0) {
                    child1.transform.position = new Vector3(child1.transform.position.x, child1.transform.position.y, child1.transform.position.z + 0.5f);
                    child2.transform.position = new Vector3(child2.transform.position.x - 0.5f, child2.transform.position.y, child2.transform.position.z);
                    child3.transform.position = new Vector3(child3.transform.position.x, child3.transform.position.y, child1.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 0.5f, child4.transform.position.y, child4.transform.position.z - 1f);
                } else if (rotateAmount==1) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 0.5f, child1.transform.position.y, child1.transform.position.z);
                    child2.transform.position = new Vector3(child2.transform.position.x, child2.transform.position.y, child2.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child1.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 1f, child4.transform.position.y, child4.transform.position.z - 0.5f);
                } else if (rotateAmount==2) {
                    child1.transform.position = new Vector3(child1.transform.position.x, child1.transform.position.y, child1.transform.position.z - 0.5f);
                    child2.transform.position = new Vector3(child2.transform.position.x + 0.5f, child2.transform.position.y, child2.transform.position.z);
                    child3.transform.position = new Vector3(child3.transform.position.x, child3.transform.position.y, child1.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 0.5f, child4.transform.position.y, child4.transform.position.z + 1f);
                } else if (rotateAmount==3) {
                    child1.transform.position = new Vector3(child1.transform.position.x - 0.5f, child1.transform.position.y, child1.transform.position.z);
                    child2.transform.position = new Vector3(child2.transform.position.x, child2.transform.position.y, child2.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child1.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 1f, child4.transform.position.y, child4.transform.position.z + 0.5f);
                }
                break;
            case 3: break;
            case 4:
                if(rotateAmount==0 || rotateAmount==2) {
                    child1.transform.position = new Vector3(child1.transform.position.x, child1.transform.position.y, child1.transform.position.z + 0.5f);
                    child2.transform.position = new Vector3(child2.transform.position.x - 0.5f, child2.transform.position.y, child2.transform.position.z);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z - 1f);
                    child4.transform.position = new Vector3(child4.transform.position.x, child4.transform.position.y, child4.transform.position.z - 0.5f);
                } else if (rotateAmount==1 || rotateAmount==3) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 1f, child1.transform.position.y, child1.transform.position.z);
                    child2.transform.position = new Vector3(child2.transform.position.x + 0.5f, child2.transform.position.y, child2.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                }
                break;
            case 5:
                if(rotateAmount==0 || rotateAmount==2) {
                    child1.transform.position = new Vector3(child1.transform.position.x + 0.5f, child1.transform.position.y, child1.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 1f, child4.transform.position.y, child4.transform.position.z);
                } else if (rotateAmount==1 || rotateAmount==3) {
                    child1.transform.position = new Vector3(child1.transform.position.x - 0.5f, child1.transform.position.y, child1.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 1f, child4.transform.position.y, child4.transform.position.z);
                }
                break;
            case 6:
                if(rotateAmount==0) {
                    child2.transform.position = new Vector3(child2.transform.position.x + 0.5f, child2.transform.position.y, child2.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 0.5f, child4.transform.position.y, child4.transform.position.z - 0.5f);
                } else if (rotateAmount==1) {
                    child2.transform.position = new Vector3(child2.transform.position.x + 0.5f, child2.transform.position.y, child2.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x - 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 0.5f, child4.transform.position.y, child4.transform.position.z - 0.5f);
                } else if (rotateAmount==2) {
                    child2.transform.position = new Vector3(child2.transform.position.x - 0.5f, child2.transform.position.y, child2.transform.position.z - 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z + 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x - 0.5f, child4.transform.position.y, child4.transform.position.z + 0.5f);
                } else if (rotateAmount==3) {
                    child2.transform.position = new Vector3(child2.transform.position.x - 0.5f, child2.transform.position.y, child2.transform.position.z + 0.5f);
                    child3.transform.position = new Vector3(child3.transform.position.x + 0.5f, child3.transform.position.y, child3.transform.position.z - 0.5f);
                    child4.transform.position = new Vector3(child4.transform.position.x + 0.5f, child4.transform.position.y, child4.transform.position.z + 0.5f);
                }
                break;
        }
        //Je nach rotationsrichtung hier ++ oder --
        rotateAmount++;
        if(rotateAmount==4) {
            rotateAmount=0;
        }
        return rotateAmount; //Set the new rotateAmount
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
        if(column<0 || column>9 || row<0 || row>17) {
            Debug.Log("Invalid Placement: Out of bounds");
            return true;
        }
        return false;
    }

    //For each row, checks if it is full and proceeds accordingly
    public async Task deleteFullRows() {
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
        Manager.clearCounter += counter; //Let the GameManager know of the progress
        if(counter>0) {
            audioSource.PlayOneShot(RowClear, volume);
            if(!Manager.introductoryLevel) {
                switch (counter) {
                    case 1: Manager.playerScore+=40;
                            break;
                    case 2: Manager.playerScore+=100;
                            break;
                    case 3: Manager.playerScore+=300;
                            break;
                    case 4: Manager.playerScore+=1200;
                            break;
                }
                await speechOut.Speak("Score is "+Manager.playerScore);
            }
            while(counter>0) { //All rows above the highest fallen row are now decreased as many times as rows have been deleted
                decreaseRowsAbove(maxRow+1);
                counter--;
                maxRow--;
            }
        }
        if(!Manager.introductoryLevel || SpawnManager.introCounter>=3) {
            await Manager.traceSkyline();
            Manager.lowerHandle.Free();
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
        for(int i=9; i>=0; i--) {
            GameObject child = currentRow.transform.GetChild(i).gameObject;
            child.transform.parent = null;
            
            //child.GetComponent<PantoBoxCollider>().Disable();
            //child.GetComponent<PantoBoxCollider>().Remove();
            Destroy(child.gameObject);
            
        }
        allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().Remove();
        //if (allRowsParent.transform.GetChild(0).transform.childCount > 0)
        //{
            allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().CreateObstacle();
            allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().Enable();
        //}
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

    public static void cleanUpRows() {
        GameObject parentRow;
        int children;
        for(int row=0; row<h; row++) {
            parentRow = allRowsParent.transform.GetChild(row).gameObject;
            children = parentRow.transform.childCount;
            for(int block=0; block<children; block++) {
                Destroy(parentRow.transform.GetChild(block).gameObject);
            }
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
        for(int i=block.transform.childCount-1; i>=0; i--) {
            xPosRelative = (Mathf.Round(block.transform.GetChild(i).transform.position.x * 2f) / 2f);
            zPosRelative = (Mathf.Round(block.transform.GetChild(i).transform.position.z * 2f) / 2f);
            column = (int)(2 * (xPosRelative+offsetX));
            row = (int)(2 * (zPosRelative+offsetZ));
            updateTagName(column, row, block.transform.GetChild(i));
            //block.transform.GetChild(i).gameObject.GetComponent<PantoBoxCollider>().CreateObstacle();
            //block.transform.GetChild(i).gameObject.GetComponent<PantoBoxCollider>().Enable();
            parentRow = GameObject.Find("Row"+row);
            block.transform.GetChild(i).transform.SetParent(parentRow.transform);            
        }
        Destroy(block);
        allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().Remove();
        if (allRowsParent.transform.GetChild(0).transform.childCount >0)
        { 
            allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().CreateObstacle();
            allRowsParent.transform.gameObject.GetComponent<PantoCompoundCollider>().Enable();
        }
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
            //Put the whole function into placeBlock and reuse PosRelative?
            xPosRelative = (Mathf.Round(child.transform.position.x*2f)/2f);
            zPosRelative = (Mathf.Round(child.transform.position.z*2f)/2f);
            column=(int)(2*(xPosRelative+offsetX));
            row=(int)(2*(zPosRelative+offsetZ));
            if(checkPosition(column, row)) {
                Debug.Log("Invalid Placement: Position already occupied");
                return false;
            }
            //Check for this child if it is on bottom row or below is another block
            if(row==0 || checkPosition(column, row-1)) {
                surroundings++;
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
}