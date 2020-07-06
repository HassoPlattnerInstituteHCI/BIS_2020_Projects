using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, -27.5f, -10f);
    }
}
