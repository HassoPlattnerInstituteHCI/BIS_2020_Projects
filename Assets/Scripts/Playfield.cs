using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    // The Grid itself
    public static int w = 10;
    public static int h = 18;
    public static Transform[,] grid = new Transform[w, h];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector3 roundVec3(Vector3 v) {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
    }

    public static bool insideBorder(Vector3 pos) {
        return ((int)pos.x >= -2.5 && (int)pos.x < -2.5*(w/2) && (int)pos.z >= -4.5 && (int)pos.z <= 4.5);
    }

    public static void deleteRow(int y) {
        for (int x = 0; x < w; ++x) {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public static void decreaseRow(int y) {
        for (int x = 0; x < w; ++x) {
            if (grid[x, y] != null) {
                // Move one towards bottom
                grid[x, y-1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y-1].position = new Vector3(0, 0, (float)-0.5);
            }
        }
    }

    public static void decreaseRowsAbove(int y) {
        for (int i = y; i < h; ++i)
            decreaseRow(i);
    }

    public static bool isRowFull(int y) {
        for (int x = 0; x < w; ++x)
            if (grid[x, y] == null)
                return false;
        return true;
    }

    public static void deleteFullRows() {
        for (int y = 0; y < h; ++y) {
            if (isRowFull(y)) {
                deleteRow(y);
                decreaseRowsAbove(y+1);
                --y;
            }
        }
    }
}
