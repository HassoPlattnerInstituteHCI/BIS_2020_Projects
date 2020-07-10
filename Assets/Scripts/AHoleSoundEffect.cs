using UnityEngine;
using SpeechIO;
using System.Threading.Tasks;

public class AHoleSoundEffect : MonoBehaviour
{
    public AudioClip blaBla;
    public AudioSource audioSource;
    GameManager gameManager;

    void Start()
    {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
    }




    public void StopPlayback()
    {
        audioSource.Stop();
    }

}