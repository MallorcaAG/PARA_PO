using UnityEngine;

public class UICameraOrientation : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
