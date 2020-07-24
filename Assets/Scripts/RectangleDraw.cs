using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PantoDrawing
{
public class RectangleDraw : MonoBehaviour
{
    public static void CreateRectanglePoints(LineRenderer lineRenderer)
    {
        Vector3[] line = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(line);
        float x_max = -10000, x_min = 10000, z_max = -10000, z_min = 10000;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            if(line[i].x < x_min) x_min = line[i].x;
            if(line[i].x > x_max) x_max = line[i].x;
            if(line[i].z < z_min) z_min = line[i].z;
            if(line[i].z > z_max) z_max = line[i].z;
        }
        Debug.Log(x_min+ " "+ x_max+ " "+ z_min+ " "+ z_max);
        int count = lineRenderer.positionCount/4;
        float x_step = (x_max-x_min)/(count);
        float z_step = (z_max-z_min)/(count);
        Vector3[] newPositions = new Vector3[count*4];
        for (int i = 0; i < count; i++)
        {
            newPositions[i] = new Vector3(x_min+x_step*i,.1f,z_min);
            newPositions[i + count] = new Vector3(x_max,.1f,z_min+z_step*i);
            newPositions[i + 2 * count] = new Vector3(x_max-x_step*i,.1f,z_max);
            newPositions[i + 3 * count] = new Vector3(x_min,.1f,z_max-z_step*i);
        }
        lineRenderer.positionCount = count*4;
        lineRenderer.SetPositions(newPositions);   
    }
}
}
