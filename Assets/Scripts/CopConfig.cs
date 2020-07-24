using UnityEngine;
using DualPantoFramework;

namespace gta{
/// <summary>
/// Enemy configuration for different difficulities.
/// </summary>
[CreateAssetMenu(fileName = "Cop", menuName = "ScriptableObjects/CopConfig", order = 1)]
public class CopConfig : ScriptableObject
{
    public int health = 100;
    public float speed = 0.001f;
    public bool CSGoPlayer = true;
    public float turnSpeed = 0.1f;
    public float randomStepSpeed = 0.01f;
    public float fieldOfView = 30;
    public float timeTillSeek = 2f;
    public float inaccuracy = 0.2f;
    public bool returnsFireOnAttack = true;
    public bool attackPlayerAtStart = true;
}
}