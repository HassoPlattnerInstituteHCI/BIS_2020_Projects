using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator changeAnimation;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    //private PantoHandle lowerHandle;

    public int direction = -1;
    float xpos;
    float time;

    // Start is called before the first frame update
    async void Awake()
    {
        time = Time.time;
        xpos = transform.position.x;
        rigid = GetComponent<Rigidbody2D>();
        changeAnimation = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        //Invoke("Think", 5);
        //lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        //await lowerHandle.MoveToPosition(transform.position, 0.2f);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Moving 
        rigid.velocity = new Vector2(direction, rigid.velocity.y);
        if((Time.time - time) > .1f)
        {
            if (transform.position.x == xpos)
            {
                direction = direction * -1;
                spriteRenderer.flipX = direction == 1;
            }
            else
            {
                xpos = transform.position.x;
            }
                
            time = Time.time;
        }


        if(transform.position.y < -33)
        {
            DeActive();
        }
        //transform.position = lowerHandle.HandlePosition(transform.position);

        //Platform checking
        Vector2 frontVec = new Vector2(rigid.position.x + direction * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            if (rayHit.distance < 0.1f)
            {
                Turning();
            }
        }
    }

    void Think()
    {
        //nextMove = Random.Range(-1, 2);

        // Changing the direction of the animation
        //changeAnimation.SetInteger("MovingSpeed", nextMove);
        //if (nextMove != 0)
        //{
        //    spriteRenderer.flipX = nextMove == 1;
        //}

        //float selfMoveTime = Random.Range(2f, 5f);
        //Invoke("Think", selfMoveTime);
    }

    void Turning()
    {
        direction = direction * -1;
        spriteRenderer.flipX = direction == 1;

    }

    public void OnDamaged()
    {
        //Chaning color of the player after getting damage
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //Change the animation in y position
        spriteRenderer.flipY = true;

        //Disable collider
        capsuleCollider.enabled = false;

        // 
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //
        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
