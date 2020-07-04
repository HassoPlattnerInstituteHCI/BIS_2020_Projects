using UnityEngine;
using SpeechIO;

public class TelephoneSoundEffect : MonoBehaviour
{
    public AudioClip ringClip;

    SpeechOut speechOut;

    public AudioSource audioSource;
    //public AudioSource rollSource;

    //private GameObject TelephoneBox1;

    //public SpeechOut speechOut = new SpeechOut();

    void Start()
    {

        speechOut = new SpeechOut();
        //audioSource = GetComponent<AudioSource>();
        //rollSource = gameObject.AddComponent<AudioSource>();
        //rollSource.loop = true;
        //rollSource.clip = rollClip;
    }


    public void startPhoneRing(){

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = ringClip;
        audioSource.Play();
    }

    public void startPhoneTalks(int level){
        if(level == 1){
            speechOut.Speak("Yo Claude, it's Johnny Zoo. Congratulations on breaking out of prison!");
        }
    }





    public void StopPlayback()
    {
        audioSource.Stop();
    }

}