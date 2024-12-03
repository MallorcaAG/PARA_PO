using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SasakayIndicatorDestroyer : MonoBehaviour
{
    private GameObject myObject;

    // Start is called before the first frame update
    void Start()
    {
        myObject = transform.parent.gameObject;
    }

    public void DestroyTrigger(Component sender, object data)
    {
        GameObject npc = (GameObject)data;

        if(npc == myObject)
        {
            Destroy(gameObject);
        }
    }

}
