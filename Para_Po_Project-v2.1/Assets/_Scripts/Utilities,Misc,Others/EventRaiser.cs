using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRaiser : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;


    public void invokeEvent()
    {
        gameEvent.Raise(this, 0);
    }
}
