using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;
using DualPantoFramework;

public class AHoleSoundEffect : MonoBehaviour
{
    public AudioClip blaBlaClip;
    public AudioSource audioSource;
    GameManager gameManager;

    void Start()
    {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));

    }

    public void startBlaBla(){
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = blaBlaClip;
        audioSource.Play();
    }

    public void StopPlayback()
    {
        audioSource.Stop();
    }

}