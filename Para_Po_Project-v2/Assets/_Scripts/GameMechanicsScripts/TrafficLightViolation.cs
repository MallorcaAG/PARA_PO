using UnityEngine;

public class TrafficLightViolation : MonoBehaviour
{
    [SerializeField] private GameEvent onRedLightViolation;

    private bool beating = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            beating = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && beating)
        {
            onRedLightViolation.Raise(this, 0);
        }
    }

    private void OnDisable()
    {
        beating = false;
    }

}
