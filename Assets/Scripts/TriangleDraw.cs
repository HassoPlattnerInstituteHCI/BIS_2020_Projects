using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PantoDrawing
{
public class TriangleDraw : MonoBehaviour
{
    public static void CreateTrianglePoints(LineRenderer lineRenderer)
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
        Debug.Log(x_min+" "+ x_max+" "+ z_min+" "+ z_max);
        int count = lineRenderer.positionCount/3;
        float x_step = (x_max-x_min)/(count);
        float x_mid = (x_max+x_min)/2;
        float x_mid_step = (x_max-x_mid)/(count);
        float z_step = (z_max-z_min)/(count);
        float z_mid = (z_max+z_min)/2;
        Vector3[] newPositions = new Vector3[count*3];
        for (int i = 0; i < count; i++)
        {
            newPositions[i] = new Vector3(x_min+x_step*i,.1f,z_min);
            newPositions[i + count] = new Vector3(x_max-x_mid_step*i,.1f,z_min+z_step*i);
            newPositions[i + 2 * count] = new Vector3(x_mid-x_mid_step*i,.1f,z_max-z_step*i);
        }
        lineRenderer.positionCount = count*3;
        lineRenderer.SetPositions(newPositions);   
    }
}
}
