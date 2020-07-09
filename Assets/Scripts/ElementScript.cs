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



        // Update is called once per frame
        void Update()
        {

        }

    }
}
