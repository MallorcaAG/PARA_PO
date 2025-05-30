using UnityEngine;

public class EventRaiser : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;


    public void invokeEvent()
    {
        gameEvent.Raise(this, 0);
    }
}
