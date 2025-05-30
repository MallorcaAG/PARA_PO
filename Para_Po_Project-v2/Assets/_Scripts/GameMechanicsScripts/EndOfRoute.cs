using UnityEngine;

public class EndOfRoute : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerEnters;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onPlayerEnters.Raise(this, other.gameObject);
        }
    }
}
