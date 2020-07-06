using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject mushroom;
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioCollision;
    public AudioClip audioDie;
    public AudioClip audioCoin;
    public AudioClip audioLevelCom;
    public AudioClip audioWalk;

    //private PantoHandle upperHandle;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator changeAnimation;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    async void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        changeAnimation = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        ////await upperHandle.MoveToPosition(transform.position, 0.2f);
    }


    void Update()
    {
        //transform.position = upperHandle.HandlePosition(transform.position);
        // upperHandle.MoveToPosition(transform.position, 0.2f);

        // Jumping Movement
        if (//upperHandle.GetPosition().y > 3 || 
            Input.GetButtonDown("Jump") && !changeAnimation.GetBool("isJumping"))
        {
            //Debug.Log(upperHandle.GetPosition().x + " " + upperHandle.GetPosition().y + " " + upperHandle.GetPosition().z);
            //Debug.Log(upperHandle.GetRotation());
            
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            changeAnimation.SetBool("isJumping", true);
            PlaySound("Jump");
            audioSource.Play();
        }

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

        
        // Changing animation walk to stand stand to walk
        if(Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            changeAnimation.SetBool("isWalking", false);
        } else
        {
            changeAnimation.SetBool("isWalking", true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            // Player attacks
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("Attack");
                audioSource.Play();
            } else // Player gets damage
            {
                OnDamaged(collision.transform.position);
                PlaySound("Damaged");
                audioSource.Play();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            // Point
            gameManager.stagePoint += 100;
            // collects coin
            collision.gameObject.SetActive(false);
            PlaySound("Coin");
            audioSource.Play();
        } 
        else if(collision.gameObject.tag =="Finish")
        {
            // Next Stage
            gameManager.NextStage();
            PlaySound("Finish");
            audioSource.Play();
        }
        else if (collision.gameObject.tag == "Itembox")
        {
            collision.gameObject.GetComponent<Itembox>().gotHit();
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Mushroom")
        {
            collision.gameObject.SetActive(false);
            gameManager.HealthUp();
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
    
    void OnDamaged(Vector2 targetPosition)
    {

        // Health down
        gameManager.HealthDown();

        gameObject.layer = 11;

        // changing the player color when the player gets damage
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Raction to force
        int impulseDirection = transform.position.x - targetPosition.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(impulseDirection, 1) * 7, ForceMode2D.Impulse);

        // Animation
        changeAnimation.SetTrigger("damaged");
        Invoke("OffDamaged", 3);
        
        //Play Sound
        PlaySound("Damaged");
        audioSource.Play();
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
        audioSource.Play();

    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }


    void PlaySound(string action)
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
        }
    }
}
