using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.IO;
using UnityEditor.UIElements;

public class TrackerLogic : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float constantTime;
    Vector3 oldPlayerPosition;
    float PlayerDistance;

    // Start is called before the first frame update
    void Start()
    {
        oldPlayerPosition = GameObject.FindObjectOfType<Player>().transform.position;
        PlayerDistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Positionsberechnung des Trackers

        //falschfahren (also falsche Richtung) muss noch irgendwie miteingerechnet werden und der Tracker springt noch merkwürdig herum
        Vector3 playerPosition = pathCreator.path.GetClosestPointOnPath(GameObject.FindObjectOfType<Player>().transform.position);
        float progress = Vector3.Distance(playerPosition, oldPlayerPosition);
        PlayerDistance += progress;
        float distanceToTracker = PlayerDistance + constantTime * progress / Time.deltaTime;
        Debug.Log(progress);
        //Debug.Log(constantTime * Time.deltaTime * trackSpeed);
        oldPlayerPosition = playerPosition;
        transform.position = pathCreator.path.GetPointAtDistance(distanceToTracker, endOfPathInstruction);

        //Berechnung der It-Handle-Position

        //Hier fehlt auch noch der Fall des falschfahrens und bisher wird die Lenkung nur für rechts berechnet
        float m;
        Vector3 orthogonal;
        Vector3 directional = GameObject.FindObjectOfType<Player>().transform.up;
        if (directional.x == 0)
        {
            orthogonal.x = 1;
            orthogonal.y = 0;
            orthogonal.z = 0;
            m = playerPosition.x - transform.position.x;
        }
        else
        {
            orthogonal.x = directional.z / directional.x;
            orthogonal.y = 0;
            orthogonal.z = 1;

            m = ((((-directional.z / directional.x) * transform.position.x) - playerPosition.x) + transform.position.z - playerPosition.z) / (((directional.z / directional.x) * orthogonal.x) - orthogonal.z);
        }
        float itPos = Vector3.Distance(transform.position, transform.position + m * orthogonal);
    }
}

