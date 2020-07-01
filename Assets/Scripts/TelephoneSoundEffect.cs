using UnityEngine;

public class TelephoneSoundEffect : MonoBehaviour
{
    public AudioClip ringClip;

    public AudioSource audioSource;
    //public AudioSource rollSource;

    private GameObject TelephoneBox1;

    //public SpeechOut speechOut = new SpeechOut();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //rollSource = gameObject.AddComponent<AudioSource>();
        //rollSource.loop = true;
        //rollSource.clip = rollClip;
    }


    public void startPhoneRing(AudioClip ringClip, GameObject phoneBox){
        audioSource.clip = ringClip;
        audioSource.Play();
    }





    public void StopPlayback()
    {
        audioSource.Stop();
    }

}