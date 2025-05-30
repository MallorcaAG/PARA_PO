using UnityEngine;

public class SasakayIndicatorDestroyer : MonoBehaviour
{
    public GameObject sasakayIndicator;
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

    public void UIHider(Component sender, object data)
    {
        bool isMaxCapacity = (bool)data;

        if(isMaxCapacity)
        {
            sasakayIndicator.SetActive(false);
        }
        else
        {
            sasakayIndicator.SetActive(true);
        }

    }

}
