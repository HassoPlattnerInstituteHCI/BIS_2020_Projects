using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;
using System;

namespace dualLayouting 
{
    public class ElementScript : MonoBehaviour
    {
        public AudioClip creationSound;
        public AudioClip deletionSound;
        public bool recentlyCreated = true;
        private List<Vector3> dimensions = new List<Vector3>
                {
                    new Vector3 (3f,0.1f,2f),
                    new Vector3 (2f,0.1f,3f),
                    new Vector3 (1f,0.1f,4f),
                    new Vector3 (2f,0.1f,2f)
                };
        
        // TODO
        public string description;

        private AudioSource audioSource;

        // Start is called before the first frame update      
        async void Start()
        {
            recentlyCreated = true;
            Debug.Log("new element created");   
            Debug.Log(recentlyCreated);
            System.Random random = new System.Random();
            int nextIndex = random.Next(dimensions.Count);
            transform.localScale = dimensions[nextIndex];
            audioSource = GetComponent<AudioSource>();
            GetComponentInChildren<TextMesh>().text = name;
            await Task.Delay(10000);
            Debug.Log("afd");
            recentlyCreated = false;
        }

        public void PlayCreateSound() {
            //why does this but work but not the same way as below?
            GetComponent<AudioSource>().PlayOneShot(creationSound);         
        }

        public void Delete() {
            audioSource.clip = deletionSound;
            audioSource.Play();
            Destroy(gameObject, deletionSound.length);
        }

        public Vector3 TopPosition() {
            Vector3 center = transform.position;
            Vector3 size = transform.localScale * 0.5f;
            size.x = 0;
            size.y = 0;

            return center + size;
        }

        public Vector3 BottomPosition() {
            Vector3 center = transform.position;
            Vector3 size = transform.localScale * 0.5f;
            size.x = 0;
            size.y = 0;

            return center - size;
        }

        public Vector3 LeftPosition() {
            Vector3 center = transform.position;
            Vector3 size = transform.localScale * 0.5f;
            size.z = 0;
            size.y = 0;

            return center - size;
        }

        public Vector3 RightPosition() {
            Vector3 center = transform.position;
            Vector3 size = transform.localScale * 0.5f;
            size.z = 0;
            size.y = 0;

            return center + size;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mePos = GameObject.Find("Panto").GetComponent<UpperHandle>().GetPosition();
            Debug.DrawLine(Vector3.zero, mePos, Color.red);
            Vector3 elemPos = transform.position;
            Debug.DrawLine(Vector3.zero, elemPos, Color.red);
            Vector3 direction = mePos - elemPos; 
            Debug.DrawLine(elemPos, mePos, Color.blue);
            GetComponent<LinearForceField>().direction = direction; 
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag != "Element") {
                return;
            }

            if (other.gameObject.GetComponent<ElementScript>().recentlyCreated || recentlyCreated) {
                return;
            }
            
            if (other.gameObject != GameObject.Find("Panto").GetComponent<AppManager>().GetSelectedElement()){
                 return;
            }

            Debug.Log(gameObject.name + ": " + recentlyCreated);
            Debug.Log(other.gameObject.name + ": " + other.gameObject.GetComponent<ElementScript>().recentlyCreated);
            GameObject panto = GameObject.Find("Panto");
            panto.GetComponent<AppManager>().OnFirstCollision();
        }
    }
}
