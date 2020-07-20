﻿using UnityEngine;
using UnityEngine.AI;
using DualPantoFramework;

public class EnemyLogic : MonoBehaviour
{
    public Transform target;
    public float seekingDistance = 1f;
    public EnemyConfig config;
    bool foundPlayer = false;
    float timeToFind;
    Vector3 lastSeenPosition;
    NavMeshAgent agent;
    AudioSource audioSource;
    public AudioClip sirensClip;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        if (config.attackPlayerAtStart)
        {
            lastSeenPosition = target.position;
            foundPlayer = true;
        }
        GetComponent<Health>().maxHealth = config.health;
    }

    void Update()
    {

        agent.SetDestination(lastSeenPosition);
        Quaternion lookRotation = Quaternion.LookRotation(lastSeenPosition - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, config.turnSpeed);
    }

}
