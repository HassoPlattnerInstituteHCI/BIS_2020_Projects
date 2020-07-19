using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace MarioKart
{
    public class Goal : MonoBehaviour
    {
        private float distance = 5;
        private bool lap = true;
        private int lapcount = 0;
        public PathCreator pathCreator;
        public int MaxLaps;
        public GameObject checkpoint;
          
        public void NextCheckpoint()
        {
            if (pathCreator.path.length-2 <= distance)
            {
                lap = true;
            }
            else
            {
                distance += 1;
                Instantiate(checkpoint, pathCreator.path.GetPointAtDistance(distance), pathCreator.path.GetRotationAtDistance(distance));
            }
        }

        public void NextLap()
        {
            if (lap)
            {
                lapcount++;
                if (lapcount == MaxLaps + 1)
                {
                    Victory();
                }
                else
                {
                    distance = 2;
                    lap = false;
                    NextCheckpoint();
                }             
            }
        }

        void Victory()
        {
            //nächstes level laden/ siegerehrung?
            Debug.Log("Victory");
            Destroy(gameObject);
        }
    }
}
