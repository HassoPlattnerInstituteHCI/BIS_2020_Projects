using UnityEngine;
using SpeechIO;

public class PlayerSoundEffect : MonoBehaviour
{
    public AudioClip wasted;

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

    public void playWasted(){
        Debug.LogError("No TelephoneSoundsEffect component found.");

        audioSource.loop = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = wasted;
        audioSource.Play();
    }

}
