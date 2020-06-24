using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 1.0f; 
    // The enemy will patrol between these points;
    public Vector3[] path;
    // The part of the path the enemy is currently headed to
    private int currentPathTargetIndex = 0;
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position == getCurrentTarget())
        {
            currentPathTargetIndex = (currentPathTargetIndex + 1) % path.Length;
        }

        moveTowardsTarget();
    }

    void moveTowardsTarget()
    {
        gameObject.transform.position =
            Vector3.MoveTowards(gameObject.transform.position, getCurrentTarget(), Time.deltaTime * speed);
    }

    Vector3 getCurrentTarget()
    {
        return path[currentPathTargetIndex];
    }
}
