using UnityEngine;

public class LaneReader : MonoBehaviour
{
    [SerializeField] private float cooldown = 20f;
    [SerializeField] private GameEvent onCounterflowingDetected;

    private bool running;
    private float targetTime;

    private void Start()
    {
        resetTimer();
    }

    private void Update()
    {
        bool sendData = false;

        if(running)
        {
            sendData = Timer();
        }

        if(sendData)
        {
            onCounterflowingDetected.Raise(this, gameObject);
            running = false;
            sendData = false;
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if (obj.GetComponents<Waypoint>() == null)
        {
            return;
        }


        if(obj.TryGetComponent<VehicleWaypoint>(out VehicleWaypoint o))
        {
            if (o.isCorrectLane())
            {
                resetTimer();
                running = false;
            }
            else
            {
                running = true;
            }
        }
        else if(obj.TryGetComponent<Waypoint>(out Waypoint p))
        {
            running = true;
        }
 
    }

    private bool Timer()
    {
        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            resetTimer();
            return true;
        }

        return false;
    }

    private void resetTimer()
    {
        targetTime = cooldown;
    }
}
