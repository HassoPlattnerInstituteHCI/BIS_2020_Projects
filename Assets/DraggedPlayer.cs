using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;

namespace MarioKart
{
    public class DraggedPlayer : MonoBehaviour
    {
        // Start is called before the first frame update
        private DualPantoSync pantoSync;
        private PantoHandle handle;
        public float deadZone = 0.1f;
        public float maxSpeed = 5.0f;
        private Rigidbody rigidbody;
        private AudioSource motorSound;
        void Start()
        {
            pantoSync = GameObject.Find("Panto").GetComponent<DualPantoSync>();
            handle = pantoSync.GetComponent<UpperHandle>();
            rigidbody = GetComponent<Rigidbody>();
            motorSound = GetComponent<AudioSource>();

            if (!pantoSync.debug)
            {
                handle.SwitchTo(gameObject, 0.2f);
                handle.FreeRotation();
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MoveToHandle();
            SetVolume();
        }

        void MoveToHandle()
        {
            Vector3 movement = GetVector();
            if (movement.magnitude < deadZone)
            {
                return;
            }
            rigidbody.velocity = movement * maxSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);
        }

        Vector3 GetVector()
        {
            Vector3 handlePos = handle.GetPosition();
            return handlePos - transform.position;
        }

        void SetVolume()
        {
            float speed = rigidbody.velocity.magnitude / maxSpeed;
            motorSound.volume = speed;
        }
    }
}