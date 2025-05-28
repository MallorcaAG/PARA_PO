using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSimulationManager : MonoBehaviour
{
    [SerializeField] private NPCCount npcs;

    // Start is called before the first frame update
    void Start()
    {
        npcs.ResetCurrentNPCValues();
    }

    
}
