using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    Rigidbody2D rigid;
    private PantoHandle upperHandle;
    SpriteRenderer spriteRenderer;
    Animator changeAnimation;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    async void Awake()
    {
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        changeAnimation = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        //Debug.Log(HandletoPlayer(upperHandle.GetPosition()));
        await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.2f);
    }

    // Update is called once per frame
    async void Update()
    {
        transform.position = HandletoPlayer(upperHandle.HandlePosition(PlayertoHandle(transform.position)));
        await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.1f);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    Vector3 HandletoPlayer(Vector3 handlepos)
    {
        return new Vector3(handlepos.x - 70, handlepos.z - 20, 0);
    }

    Vector3 PlayertoHandle(Vector3 playerpos)
    {
        return new Vector3(playerpos.x + 70, 0, playerpos.y + 20);
    }
}
