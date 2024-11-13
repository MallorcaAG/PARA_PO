using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RandomMovementSpeed : MonoBehaviour
{
    [SerializeField] private VehicleAINavigator nav;
    [SerializeField] private VehicleAISensors sensor;
    [SerializeField] private Vector2 BaseSpeedToSensorDistanceRatio;
    [SerializeField] private Vector2 MinMaxRandSpeed;
    [SerializeField] private Vector2 MinMaxRandSensor;

    float baseSpeed, sensorDistance;

    // Start is called before the first frame update
    [SerializeField]
    private void Awake()
    {
        nav = GetComponent<VehicleAINavigator>();
        sensor = GetComponent<VehicleAISensors>();

        int n = Random.Range(0, 2);
        //Debug.Log(n);
        switch(n)
        {
            case 0:
                baseSpeed = Random.Range(MinMaxRandSpeed.x, MinMaxRandSpeed.y) + ((float)Random.Range(-100, 101) / 100);
                //Debug.Log(baseSpeed);
                nav.setBaseSpeed(baseSpeed);
                sensor.setMaxDistance(ratioD(BaseSpeedToSensorDistanceRatio.x, BaseSpeedToSensorDistanceRatio.y, baseSpeed));
                break;
            case 1:
                sensorDistance = Random.Range(MinMaxRandSensor.x, MinMaxRandSensor.y) + ((float)Random.Range(-100, 101) / 100);
                //Debug.Log(sensorDistance);
                sensor.setMaxDistance(sensorDistance);
                nav.setBaseSpeed(ratioC(BaseSpeedToSensorDistanceRatio.x, BaseSpeedToSensorDistanceRatio.y, sensorDistance));
                break;
            default:
                break;
        }
    }

    private float ratioD(float A, float B, float C)
    {
        /*
        Enter A, B and C to find D.
        The calculator shows the steps and solves for 
            D = C * (B / A)
        */
        float D = C * (B / A);

        return D;
    }

    private float ratioC(float A, float B, float D)
    {
        /*
        Enter A, B and D to find C.
        The calculator shows the steps and solves for 
            C = D * (A / B)
        */
        float C = D * (A / B);

        return C;
    }
}
