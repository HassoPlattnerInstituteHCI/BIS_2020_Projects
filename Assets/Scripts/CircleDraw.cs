using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PantoDrawing
{
public class CircleDraw : MonoBehaviour
{
    public static void CreateCircle(LineRenderer line)
    {
        Vector3 center = GetCircleCenter(line);
        Vector3 radius = GetCircleRadius(line, center);
        CreateCirclePoints(line, center, (radius.x + radius.z) / 2);
    }

    static void CreateCirclePoints (LineRenderer line, Vector3 center, float radius)
    {
        float x;
        float y = .1f;
        float z;
    
        float angle = 20f;
    
        for (int i = 0; i < line.positionCount; i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

            line.SetPosition (i,new Vector3(x+center.x,y,z+center.z));
                
            angle += (360f / line.positionCount);
        }
    }

    static Vector3 GetCircleCenter(LineRenderer line)
    {
        float sumX = 0, sumZ = 0;
        Vector3[] linePos = new Vector3[line.positionCount];
        line.GetPositions(linePos);
        for (int i = 0; i < line.positionCount; i++){
            sumX += linePos[i].x;
            sumZ += linePos[i].z;
        }
        return new Vector3(sumX/line.positionCount, .1f, sumZ/line.positionCount);
    }

    static Vector3 GetCircleRadius(LineRenderer line, Vector3 center)
    {
        float sumX = 0, sumZ = 0;
        Vector3[] linePos = new Vector3[line.positionCount];
        line.GetPositions(linePos);
        for (int i = 0; i < line.positionCount; i++){
            sumX += Mathf.Abs(linePos[i].x-center.x);
            sumZ += Mathf.Abs(linePos[i].z-center.z);
        }
        return new Vector3(sumX/line.positionCount*1.56f, .1f, sumZ/line.positionCount*1.56f);
    }
}
}
