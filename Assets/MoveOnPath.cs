using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace MarioKart
{
    public class MoveOnPath : MonoBehaviour
    {
        public PathCreator pathCreator;
        public LayerMask layerMask;

        void Update()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Transform objectHit = hit.transform;

                // Do something with the object that was hit by the raycast.
                transform.position = GetClosestPoint(hit.point);
                GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }

        Vector3 GetClosestPoint(Vector3 point)
        {
            return pathCreator.path.GetClosestPointOnPath(point);
        }
    }
}