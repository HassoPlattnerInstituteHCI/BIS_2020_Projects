using ClipperLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public GameManager gameManager;

    public float maxSpeed;
    public float jumpPower;
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
    public AudioClip audioGround;

    private PantoHandle upperHandle;
    private bool frozen;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator changeAnimation;
    CapsuleCollider2D capsuleCollider;

    float startpos;
    Vector3 safepos;
    float lasttime;
    private void Start()
    {
        startpos = transform.position.y;
        frozen = false;
    }

    async void Awake()

    {

        lasttime = Time.time;
        upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        changeAnimation = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.02f);
    }


    async void Update()
    {
        if (frozen)
        {
            return;
        }

        Vector3 goal = HandletoPlayer(upperHandle.HandlePosition(PlayertoHandle(transform.position)));
        Vector3 direction = (goal - transform.position);
        movePlayer(direction);

        handlePantoJump(direction);
        handleKeyboardJump();
        handleFalling();

        // When the player stops the movement
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // Change the body direction
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        updateAnimation();
        repositionHandle(true);



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frozen)
        {
            return;
        }
        // Move by Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Right Max Speed
        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        } else if(rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing on the ground
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    changeAnimation.SetBool("isJumping", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            // rigid.velocity.y < 0 && 
            if (transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("Attack");
                Debug.Log("Attack");

            } else // Player gets damage
            {

                OnDamaged();
                PlaySound("Damaged");

            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Coin":
                gameManager.stagePoint += 100;
                collision.gameObject.SetActive(false);
                PlaySound("Coin");
                break;
            case "Finish":
                PlaySound("Finish");
                gameManager.NextStage();
                break;
            case "Itembox":
                collision.gameObject.GetComponent<Itembox>().gotHit();
                collision.gameObject.SetActive(false);
                PlaySound("Itembox");
                break;
            case "Mushroom":
                collision.gameObject.SetActive(false);
                gameManager.HealthUp();
                PlaySound("Mushroom");
                break;

        }
    }
    
    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint += 100;
        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Enemy died
        Enemy enemyMove = enemy.GetComponent<Enemy>();
        enemyMove.OnDamaged();
    }
    
    void OnDamaged()
    {

        // Health down
        gameManager.HealthDown();

        gameObject.layer = 11;

        // changing the player color when the player gets damage
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Animation
        changeAnimation.SetTrigger("damaged");
        Invoke("OffDamaged", 3);

        //Play Sound
        PlaySound("Damaged");

    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Chaning color of the player after getting damage
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Change the animation in y position
        spriteRenderer.flipY = true;

        //Disable collider
        capsuleCollider.enabled = false;

        // 
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("Die");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }


    Vector3 HandletoPlayer(Vector3 handlepos)
    {
        return new Vector3(handlepos.x - 74, handlepos.z - 20, 0);
    }

    Vector3 PlayertoHandle(Vector3 playerpos)
    {
        return new Vector3(playerpos.x + 74, 0, playerpos.y + 20);
    }

    async public void Reposition()
    {
        await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.001f);
    }

    void jump()
    {
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        changeAnimation.SetBool("isJumping", true);
        PlaySound("Jump");

    }

    void movePlayer(Vector2 direction)
    {
        if (direction.sqrMagnitude < 5)
        {
            rigid.AddForce(new Vector2(direction.x * 0.9f, 0), ForceMode2D.Impulse);
        }
    }

    void handlePantoJump(Vector2 direction)
    {
        if ((direction.y > 0.6) && (direction.y < 1) && (Time.time - lasttime > 0.6))
        {
            jump();
            lasttime = Time.time;
        }
    }

    void handleKeyboardJump()
    {
        if (Input.GetButtonDown("Jump") && !changeAnimation.GetBool("isJumping"))
        {
            jump();
        }
    }

    void updateAnimation()
    {
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            changeAnimation.SetBool("isWalking", false);
        }
        else
        {
            changeAnimation.SetBool("isWalking", true);
        }
    }

    async void repositionHandle(bool betterfeeling)
    {
        if (betterfeeling)
        {
            await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.1f);
        }
        else if ((HandletoPlayer(upperHandle.HandlePosition(PlayertoHandle(transform.position))) - transform.position).sqrMagnitude > 0.001)
        {
            await upperHandle.MoveToPosition(PlayertoHandle(transform.position), 0.1f);
        }
    }

    async void handleFalling()
    {
        if (transform.position.y < -33)
        {
            PlaySound("Damaged");
            gameManager.HealthDown();
            gameManager.PlayerReposition();
        }
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                break;
            case "Walk":
                audioSource.clip = audioWalk;
                break;
            case "Attack":
                audioSource.clip = audioAttack;
                break;
            case "Damaged":
                audioSource.clip = audioCollision;
                break;
            case "Coin":
                audioSource.clip = audioCoin;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "Finish":
                audioSource.clip = audioLevelCom;
                break;
            case "Itembox":
                audioSource.clip = audioItembox;
                break;
            case "Mushroom":
                audioSource.clip = audioMushroom;
                break;
            case "Ground":
                audioSource.clip = audioGround;
                break;
        }
        audioSource.Play();
    }

    public void freeze()
    {
        Debug.Log("FREEEZE");
        frozen = true;
    }

    public void unfreeze()
    {
        Debug.Log("UNFREEZE");
        frozen = false;
    }


}
