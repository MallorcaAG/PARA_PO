using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Lamppost : MonoBehaviour
{
    [SerializeField] private GameObject myLight;
    private bool isNight = false;
    private bool isPlayerNearby = false;

    private void Awake()
    {
        BoxCollider trigger = GetComponent<BoxCollider>();
        trigger.isTrigger = true;

    }

    private void Start()
    {
        myLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = true;
        UpdateLightState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        UpdateLightState();
    }

    
    public void turnOnLight()
    {
        isNight = true;
        UpdateLightState();
    }

    public void turnOffLight()
    {
        isNight = false;
        UpdateLightState();
    }

    private void UpdateLightState()
    {
        myLight.SetActive(isNight && isPlayerNearby);
    }
}
