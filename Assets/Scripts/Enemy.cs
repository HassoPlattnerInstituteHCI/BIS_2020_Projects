using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace MarioKart
{
    public class Enemy : MonoBehaviour
    {

        public PathCreator pathCreator;
        public EndOfPathInstruction end;
        float distance;
        public float speed;
        // Start is called before the first frame update
        public void Spawn()
        {
            transform.position = pathCreator.path.GetPointAtDistance(pathCreator.path.length-10);
        }

        // Update is called once per frame
        void Update()
        {
            distance += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distance, end);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distance, end);
        }

        public void SetSpeed(float newspeed)
        {
            speed = newspeed;
        }
    }
}
