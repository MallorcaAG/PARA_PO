using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horn : MonoBehaviour
{
    [SerializeField] private AudioSource hornSFX;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.F))
        {
            hornSFX.mute = false;
        }
        else
        {
            hornSFX.mute = true;
        }
    }
}
