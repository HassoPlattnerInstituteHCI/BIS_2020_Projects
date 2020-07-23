using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;

namespace MarioKart
{
    public class MovementGoal : MonoBehaviour
    {
        public PathCreation.PathCreator pathCreator;
        public GameObject waitFor;
        public float waitingRadius = 1.0f;
        public float speed;
        private float trackDistance = 0.0f;

        // Update is called once per frame
        void Update()
        {
            float targetDistance = GetTargetDistance();
            if (DoMove(targetDistance))
            {
                Move();
            }
            UpdatePosition();
        }

        float GetTargetDistance()
        {
            Vector3 difference = transform.position - waitFor.transform.position;
            return difference.magnitude;
        }

        bool DoMove(float distance)
        {
            return distance <= waitingRadius;
        }

        void Move()
        {
            float targetTrackDistance = pathCreator.path.GetClosestDistanceAlongPath(waitFor.transform.position);
            trackDistance = targetTrackDistance + waitingRadius;
        }

        void UpdatePosition()
        {
            Vector3 trackPoint = pathCreator.path.GetPointAtDistance(trackDistance);
            transform.position = trackPoint;
        }

        public void Reset()
        {
            trackDistance = 0.0f;
            UpdatePosition();
        }

        public int GetLapCount()
        {
            return (int)Mathf.Floor(trackDistance / pathCreator.path.length);
        }
    }
}