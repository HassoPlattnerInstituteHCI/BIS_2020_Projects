using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace MarioKart
{
    public class Drag : MonoBehaviour
    {
        // Start is called before the first frame update
        private PantoHandle handle;
        public float deadZone = 0.1f;
        public float speed = 5.0f;
        private Rigidbody rigidbody;
        void Start()
        {
            handle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            rigidbody = GetComponent<Rigidbody>();
            handle.SwitchTo(gameObject, 0.2f);
            handle.FreeRotation();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MoveToHandle();
        }

        void MoveToHandle()
        {
            Vector3 movement = GetVector();
            if (movement.magnitude < deadZone)
            {
                return;
            }
            rigidbody.velocity = movement * speed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);
        }

        Vector3 GetVector()
        {
            Vector3 handlePos = handle.GetPosition();
            return handlePos - transform.position;
        }
    }

}