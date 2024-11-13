using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAISensors : AISensors
{
    private VehicleAINavigator myNav;

    // Start is called before the first frame update
    void Start()
    {
        myNav = GetComponent<VehicleAINavigator>();

        saveDefaultMaxDist();
    }

    // Update is called once per frame
    void Update()
    {
        if (!sensorDetected)
        {
            myNav.Gas();
        }
        else
        {
            myNav.Brakes();
        }
    }
}
