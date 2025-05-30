using UnityEngine;

public class PodiumRotater : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 1f;
    private Transform obj;

    private void Start()
    {
        obj = gameObject.transform;    
    }

    // Update is called once per frame
    void Update()
    {
        obj.Rotate(Vector3.up * rotationSpeed);
    }
}
