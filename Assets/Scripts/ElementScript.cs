using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DualPantoFramework;

namespace dualLayouting 
{
    public class ElementScript : MonoBehaviour
    {
        public AudioClip creationSound;
        public AudioClip deletionSound;
        
        // TODO
        public string description;

        private AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            GetComponentInChildren<TextMesh>().text = name;
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

        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Element") {
                Debug.Log(other);
                Debug.Log("Elements are colliding");
                GameObject panto = GameObject.Find("Panto");
                panto.GetComponent<AppManager>().OnFirstCollision();
            }  
        }
    }
}
