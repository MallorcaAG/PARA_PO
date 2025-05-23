using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficObject : MonoBehaviour
{
    [SerializeField] private GameEvent onImpactWithPlayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            onImpactWithPlayer.Raise(this, gameObject);
        }
    }
}
