using UnityEngine;

public class TrafficLightViolation : MonoBehaviour
{
    [SerializeField] private GameEvent onRedLightViolation;

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onRedLightViolation.Raise(this, 0);
        }
    }
}
