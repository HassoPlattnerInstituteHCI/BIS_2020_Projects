using Stealth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public AudioSource hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.gameObject.tag == "Enemy")
        {
            
            hit.Play();
            collision.collider.gameObject.GetComponent<EnemyController>().TakeHit();
        }
    }
}
