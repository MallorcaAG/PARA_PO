using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeddyNavToucher : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Pedestrians"))
        {
            GameObject peddyObj = collider.gameObject;
            PedestrianAINavigator peddyNav = peddyObj.GetComponent<PedestrianAINavigator>();
            if(peddyNav.getNPCState() == 0)
            {
                peddyNav.AllowIngress(true);
            }
        }
    }
}
