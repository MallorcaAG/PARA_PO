using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFollowPlayer : MonoBehaviour
{
    [Tooltip("Player transform to follow")]
    public Transform player;

    [Tooltip("Height above the player to position the rain emitter")]
    public float heightAbovePlayer = 10f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPos = player.position;
        newPos.y += heightAbovePlayer;

        transform.position = newPos;
    }
}
