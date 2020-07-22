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

    public AudioClip[] radioList;

    void Start()
    {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));        

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

    public void startCopsRadioTalk(){
        audioSource.maxDistance = 5;
        audioSource.loop = true;

        //Make This nonloop random
        audioSource.clip = radioList[0];
        audioSource.Play();

    }


    


    public void StopPlayback()
    {
        audioSource.Stop();
    }

    
}
