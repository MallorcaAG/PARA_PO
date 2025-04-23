using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamppost : MonoBehaviour
{
    [SerializeField] private GameObject myLight;

    public void turnOnLight()
    {
        myLight.SetActive(true);
    }

    public void turnOffLight()
    {
        myLight.SetActive(false);
    }
}
