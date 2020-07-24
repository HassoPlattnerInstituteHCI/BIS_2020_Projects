using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Collections;

public class Shooting : MonoBehaviour
{
    public float maxRayDistance = 20f;
    public LayerMask hitLayers;
    // TODO: 6. A clever way of keeping track of hits might be to make the damage/second dependent on how precisely you hit the opponent, rather than having a step function hit/no hit.
    public int damage = 10;
    public float cooldown = 0.5f;
    public float startWidth = 0.1f;
    public float endWidth = 0.1f;
    public bool isUpper = true;
    public bool spotted = false;
    public AudioClip defaultClip;
    public AudioClip wallClip;
    public AudioClip hitClipMG;
    public AudioClip hitClipSniper;
    public AudioClip hitClipPump;
    private float timestamp = 0;
    private int gunnr;

    public float fireSpreadAngle = 1f;
    public Transform enemyTransform;

    AudioSource audioSource;
    SpeechOut speechOut;
    AudioClip _currentClip;
    LineRenderer lineRenderer;
    PantoHandle handle;

    AudioClip currentClip
    {
        get => _currentClip;
        set
        {
            if (_currentClip == null) _currentClip = value;
            else if (!currentClip.Equals(value))
            {
                _currentClip = value;
                audioSource.Stop();
                audioSource.clip = value;
                audioSource.Play();
            }
        }
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        speechOut = new SpeechOut();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = defaultClip;

        GameObject panto = GameObject.Find("Panto");
        if (isUpper)
        {
            handle = panto.GetComponent<UpperHandle>();
        }
        else
        {
            handle = panto.GetComponent<LowerHandle>();
        }
    }

    void Update()
    {
        //Fire();
        FireCone();
    }

    /// <summary>
    /// Fire gun with aiming assistance.
    /// </summary>
    async void FireCone()
    {
        RaycastHit hit;

        // Getting upper rotation only for player interesting
        if (isUpper)
            transform.rotation = Quaternion.Euler(0, handle.GetRotation(), 0);

        Vector3 enemyDirection = enemyTransform.position - transform.position;
        float rotationDifference = Vector3.Angle(transform.forward, enemyDirection);

        if (Mathf.Abs(rotationDifference) <= fireSpreadAngle)
        {
            if (Physics.Raycast(transform.position, enemyDirection, out hit, maxRayDistance, hitLayers))
            {
                GameObject panto = GameObject.Find("Panto");

                lineRenderer.SetPositions(new Vector3[] { transform.position, hit.point });
                lineRenderer.material.color = Color.red;
                lineRenderer.startWidth = startWidth;
                lineRenderer.endWidth = endWidth;

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    if (panto.GetComponent<GameManager>().level >= 2 && !spotted && CompareTag("Player"))
                    {
                        spotted = true;
                        GameObject e = hit.transform.gameObject;
                        PantoHandle lowerHandle = panto.GetComponent<LowerHandle>();
                        _ = speechOut.Speak("Enemy found!");
                        await lowerHandle.SwitchTo(e, 0.2f);
                    }

                    if (timestamp <= Time.time)
                    {
                        enemy.TakeDamage(damage, gameObject);
                        if (CompareTag("Player"))
                        {
                            gunnr = GameObject.Find("Panto").GetComponent<Levels>().gun;
                        }
                        else
                        {
                            gunnr = 1;
                        }
                        switch (gunnr)
                        {
                            case 1: currentClip = hitClipSniper; break;
                            case 2: currentClip = hitClipMG; break;
                            case 3: currentClip = hitClipPump; break;
                            default: break;
                        }
                        timestamp = Time.time + cooldown;
                    }
                }
                else
                {
                    currentClip = wallClip;
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, maxRayDistance, hitLayers))
            {
                GameObject panto = GameObject.Find("Panto");
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    hit.point });
                lineRenderer.material.color = Color.red;
                lineRenderer.startWidth = startWidth;
                lineRenderer.endWidth = endWidth;

                Health enemy = hit.transform.GetComponent<Health>();

                if (enemy)
                {
                    if (panto.GetComponent<GameManager>().level >= 2 && !spotted && CompareTag("Player"))
                    {
                        spotted = true;
                        GameObject e = hit.transform.gameObject;
                        PantoHandle lowerHandle = panto.GetComponent<LowerHandle>();
                        _ = speechOut.Speak("Enemy found!");
                        await lowerHandle.SwitchTo(e, 0.2f);
                    }
                    if (timestamp <= Time.time)
                    {
                        enemy.TakeDamage(damage, gameObject);
                        if (CompareTag("Player"))
                        {
                            gunnr = GameObject.Find("Panto").GetComponent<Levels>().gun;
                        }
                        else
                        {
                            gunnr = 1;
                        }
                        switch(gunnr)
                        {
                            case 1: currentClip = hitClipSniper; break;
                            case 2: currentClip = hitClipMG; break;
                            case 3: currentClip = hitClipPump; break;
                            default: break;
                        }
                        timestamp = Time.time + cooldown;
                    }
                }
                else
                {
                    currentClip = wallClip;
                }
            }
            else
            {
                lineRenderer.SetPositions(new Vector3[] { transform.position,
                    transform.position + transform.forward * maxRayDistance });
                lineRenderer.material.color = Color.red;
                lineRenderer.startWidth = startWidth;
                lineRenderer.endWidth = endWidth;
                currentClip = defaultClip;
            }

        }
    }
}
