using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Player : MonoBehaviour
{
    public PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;
    public float speedModifier;
    public GameObject tracker;
    public GameObject trackerPos;
    private GameObject panto;
    private PantoHandle meHandle;
    private PantoHandle itHandle;
    public GameObject desired;
    float hasPowerup = 0;
    private Rigidbody rigidBody;
    public float maxSpeed = 10.0f;
    public float maxMeDistance = 1.0f;
    // [Range(0.0f, 1.0f)]
    public float handleSpeed = 1.0f;
    private PauseManager pauseManager;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = pathCreator.path.GetPointAtDistance(0, endOfPathInstruction);
        panto = GameObject.Find("Panto");
        meHandle = panto.GetComponent<UpperHandle>();
        itHandle = panto.GetComponent<LowerHandle>();
        rigidBody = GetComponent<Rigidbody>();
        pauseManager = GetComponent<PauseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseManager.isPaused)
        {
            return;
        }

        itHandle.SwitchTo(tracker, handleSpeed);

        MoveTracker();
        RotateToMe();
        ApplyForce();
    }

    void MoveTracker()
    {
        Vector3 closest = pathCreator.path.GetClosestPointOnPath(trackerPos.transform.position);
        tracker.transform.position = closest;
    }

    void RotateToMe()
    {
        float myAngle = transform.rotation.eulerAngles.y;
        float angle = meHandle.GetRotation();
        float angleDiff = angle - myAngle;
        Vector3 newVel = Quaternion.Euler(0, angleDiff, 0) * rigidBody.velocity;
        transform.eulerAngles = new Vector3(0, angle, 0);
        rigidBody.velocity = newVel;
    }

    float GetAccelleration()
    {
        Vector3 normal = transform.rotation * Vector3.forward;
        Vector3 desired = meHandle.GetPosition() - transform.position;
        desired = Vector3.ClampMagnitude(desired, maxMeDistance);
        float factor = Vector3.Dot(normal, desired);
        MoveMeTo(transform.position + desired);
        return factor;
    }

    async void MoveMeTo(Vector3 position)
    {
        desired.transform.position = position;
        await meHandle.SwitchTo(desired, handleSpeed);
        meHandle.Free();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerupzone") && hasPowerup != 0)
        {
            hasPowerup = Random.Range(0, 2);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }

    void ApplyForce()
    {
        float speedFactor = GetAccelleration();
        speedFactor = Mathf.Clamp(speedFactor, -maxSpeed, maxSpeed);
        Vector3 speed = transform.rotation * Vector3.forward * speedFactor * maxSpeed;
        rigidBody.velocity = speed;
    }
}

