using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowingHornViolation : MonoBehaviour
{
    [SerializeField] private GameEvent onBlowingHornViolation;

    private bool triggered = false;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || triggered)
        {
            return;
        }


        if (Input.GetKey(KeyCode.F))
        {
            if(!triggered)
            {
                onBlowingHornViolation.Raise(this, 0);
                triggered = true;
            }
        }
    }
}
