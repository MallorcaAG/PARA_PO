using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNav : MonoBehaviour
{
    [System.Serializable]
    public class DestinationInfo
    {
        public Vector3 coordinates;
        public bool reachedDestination;
    }

    public float movementSpeed = 5f;
    public float rotationSpeed = 720f;
    public float stopDistance = 0.1f;
    public DestinationInfo destinationInfo;
    private Vector3 lastPosition;


    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (transform.position != destinationInfo.coordinates)
        {
            Vector3 destinationDirection = destinationInfo.coordinates - transform.position;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                destinationInfo.reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                destinationInfo.reachedDestination = true;
            }
        }

        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        velocity.y = 0;
        lastPosition = transform.position;

        float velocityMagnitude = velocity.magnitude;
        velocity = velocity.normalized;
        float fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        float rightDotProduct = Vector3.Dot(transform.right, velocity);

        /*animator.SetFloat("horizontal", rightDotProduct);
        animator.SetFloat("Forward", fwdDotProduct);*/
    }

    public void SetDestination(Vector3 newDestination)
    {
        destinationInfo.coordinates = newDestination;
        destinationInfo.reachedDestination = false;
    }
}