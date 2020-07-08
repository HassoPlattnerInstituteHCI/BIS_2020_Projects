using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

public class TelephoneSoundEffect : MonoBehaviour
{
    public AudioClip ringClip;

    SpeechOut speechOut;

    public AudioSource audioSource;
     GameManager gameManager;

    void Start()
    {

        speechOut = new SpeechOut();
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }


    public void startPhoneRing(){

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = ringClip;
        audioSource.Play();
    }

    public async Task startPhoneTalks(int level){
        if(level == 1){
            await speechOut.Speak("Yo Claude, it's Johnny Zoo. " );//Congratulations on breaking out of prison! Welcome back to Downtown Anywhere City. Dude. This city went crazy during the time you spent in prison. You may have some trouble navigating without being able to see. I will help you out. Grab the baseball bat that I have hidden here in the telephone booth. You can use it to spot objects around you. I will call you on the other telephone booth to see if that works for you.");
            gameManager.StartLevel2();
        }

        if(level == 2){
            await speechOut.Speak("Yeah you made it!" );
            gameManager.StartLevel3();
        }
    }


    public void StopPlayback()
    {
        audioSource.Stop();
    }

}