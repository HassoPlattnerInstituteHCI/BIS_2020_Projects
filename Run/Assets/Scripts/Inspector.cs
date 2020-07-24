using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    private PantoHandle upperHandle;
    AudioSource audioSource;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioCollision;
    public AudioClip audioDie;
    public AudioClip audioCoin;
    public AudioClip audioLevelCom;
    public AudioClip audioWalk;
    public AudioClip audioItembox;
    public AudioClip audioMushroom;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GetComponent<Player>());
    }

    async void Awake()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        audioSource = GetComponent<AudioSource>();
        Debug.Log(HandletoPlayer(upperHandle.GetPosition()));
        await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.2f);
    }

    // Update is called once per frame
    async void Update()
    {
        transform.position = HandletoPlayer(upperHandle.HandlePosition(PlayertoHandle(transform.position)));
        //await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.1f);
    }

    Vector3 HandletoPlayer(Vector3 handlepos)
    {
        return new Vector3(handlepos.x - 74, handlepos.z - 20, 0);
    }

    Vector3 PlayertoHandle(Vector3 playerpos)
    {
        return new Vector3(playerpos.x + 74, 0, playerpos.y + 20);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if(collision.gameObject.tag == "Enemy")
        {
            audioSource.clip = audioAttack;
            audioSource.Play();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {

            case "Coin":
                audioSource.clip = audioCoin;
                break;
            case "Itembox":
                audioSource.clip = audioItembox;
                break;
            case "Mushroom":
                audioSource.clip = audioMushroom;
                break;
            case "Ground":
                audioSource.clip = audioCollision;
                break;

        }
        audioSource.Play();
    }

}
