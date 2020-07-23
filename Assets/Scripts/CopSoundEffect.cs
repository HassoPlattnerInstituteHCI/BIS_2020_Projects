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

    private bool shouldPlayRadio;

    public AudioClip freeze;

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
        shouldPlayRadio = true;
        StartCoroutine("PlayRandom");
        

    }

    IEnumerator PlayRandom(){

        while(shouldPlayRadio){
            int radioToPlay = Random.Range(0, radioList.Length);
            AudioClip clip = radioList[radioToPlay];
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }        

    }

    public void stopRadio(){
        shouldPlayRadio = false;
        StopPlayback();        
    }


    public void playShout(){
        StopPlayback();
        audioSource.maxDistance = 5;
        audioSource.loop = true;
        audioSource.clip = freeze;
        audioSource.Play();

    }


    


    public void StopPlayback()
    {
        audioSource.Stop();
    }

    
}
