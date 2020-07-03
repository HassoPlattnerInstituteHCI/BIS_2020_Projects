using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    // The Grid itself
    public static int w = 10;
    public static int h = 18;
    //grid of GameObjects=Small blocks?
    public static bool[,] grid = new bool[w,h];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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
