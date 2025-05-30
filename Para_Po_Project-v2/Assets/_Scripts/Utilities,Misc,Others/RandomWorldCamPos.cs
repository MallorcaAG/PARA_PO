using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomWorldCamPos : MonoBehaviour
{
    [SerializeField] private List<Transform> locations;

    private Transform my_Transform;

    private void Start()
    {
        DateTime currentTime = DateTime.Now;
        int i = currentTime.Second % locations.Count;

        Transform pos = locations[i];
        pos.parent = null;

        my_Transform = GetComponent<Transform>();
        my_Transform.position = pos.position;
        my_Transform.rotation = pos.rotation;

    }
}
