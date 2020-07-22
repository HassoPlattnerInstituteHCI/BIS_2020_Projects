using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using DualPantoFramework;
using System.Collections;

public class CopSoundEffect : MonoBehaviour
{
    SpeechOut speechOut;

    public AudioSource audioSource;
    GameManager gameManager;

    public AudioClip carDoorOpenCloses;

    void Start()
    {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));        
        //We dont need to get the audiosources here because we assign them directly in unity as its otherwise complicates with multiple sources
        //audioSource = GetComponents<AudioSource>();
        //audioSourcePolice = gameObject.AddComponent<AudioSource>();


    }

    void Update(){      
    
        
    }


    public void playCopCarArrived(){

        //Should be heard everywhere on map but still dependend on Distance to player  
        audioSource.maxDistance = 10;

        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(carDoorOpenCloses);
    }


    


    public void StopPlayback()
    {
        audioSource.Stop();
    }

    
}
