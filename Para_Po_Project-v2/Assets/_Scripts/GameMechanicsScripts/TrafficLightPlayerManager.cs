using UnityEngine;

public class TrafficLightPlayerManager : TrafficLightLaneManager
{
    [SerializeField] private GameObject straightEnd;
    [SerializeField] private GameObject leftEnd;
    [SerializeField] private GameObject rightEnd;

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(this.gameObject + " " +  collider);
    }
}
