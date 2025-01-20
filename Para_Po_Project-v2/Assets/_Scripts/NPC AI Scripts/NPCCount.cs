using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Counter", menuName = "NPC Counter")]
public class NPCCount : ScriptableObject
{
    [Header("Edit these values")]
    [SerializeField] private int maxPedestrianCount;
    [SerializeField] private int maxVehicleCount;
    [Space]
    [Header("In-game variables (Do not touch)")]
    [SerializeField] private int currentPedestrianCount;
    [SerializeField] private int currentVehicleCount;

    public void ResetCurrentNPCValues()
    {
        currentPedestrianCount = 0;
        currentVehicleCount = 0;
    }

    public void addPedestrianNPC()
    {
        currentPedestrianCount++;
    }

    public void subtractPedestrianNPC()
    {
        currentPedestrianCount--;
    }

    public bool maxPedestrianCountReached()
    {
        if(currentPedestrianCount >= maxPedestrianCount)
        {
            return true;
        }

        return false;
    }

    public void addVehicleNPC()
    {
        currentVehicleCount++;
    }

    public void subtractVehicleNPC()
    {
        currentVehicleCount--;
    }

    public bool maxVehicleCountReached()
    {
        if (currentVehicleCount >= maxVehicleCount)
        {
            return true;
        }

        return false;
    }

}
