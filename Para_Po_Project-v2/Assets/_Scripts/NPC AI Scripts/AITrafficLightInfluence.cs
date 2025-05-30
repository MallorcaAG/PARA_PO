using UnityEngine;

public class AITrafficLightInfluence : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<VehicleAISensors>(out VehicleAISensors ai))
        {
            ai.setMaxDistance(1f);
        }

        if (other.gameObject.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2))
        {
            ai2.setBaseSpeed(7f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<VehicleAISensors>(out VehicleAISensors ai))
        {
            ai.setMaxDistance(ai.getDefaultMaxDist());
        }

        if (other.gameObject.TryGetComponent<VehicleAINavigator>(out VehicleAINavigator ai2))
        {
            ai2.setBaseSpeed(ai2.getMyBaseSpeed());
        }
    }
}
