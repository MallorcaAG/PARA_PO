using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrikeController : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float steerStrength;
    [SerializeField] private float zTiltAngle = 45f;
    [SerializeField] private float handleRotationValue = 30f;
    [SerializeField] private float handleRotationSpeed = .15f;
    [SerializeField] private float tireRotationSpeed = 10000f;
    [SerializeField] private float gravity;
    [SerializeField] private float xTiltIncrement;
    [Range(0.6f,1f)][SerializeField] private float brakingFactor;

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)] private float minPitch = 1f;
    [SerializeField]
    [Range(1, 5)] private float maxPitch = 5f;

    private float moveInput, steerInput;
    private float rayLength;
    private float currentVelocityOffset;
    private Vector3 velocity;
    private RaycastHit hit;

    [Header("References")]
    [SerializeField] private Rigidbody sphereRB, TrikeBody;
    [SerializeField] private GameObject Handle;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private GameObject[] tires;

    #region Unity Functions

    private void Start()
    {
        sphereRB.transform.parent = null;
        TrikeBody.transform.parent = null;

        rayLength = sphereRB.GetComponent<SphereCollider>().radius * 4.83f + 0.2f;
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        transform.position = sphereRB.transform.position;

        velocity = TrikeBody.transform.InverseTransformDirection(TrikeBody.velocity);
        currentVelocityOffset = velocity.z / maxSpeed;
    }

    private void FixedUpdate()  
    {
        Movement();

        foreach (GameObject t in tires)
        {
            t.transform.Rotate(Vector3.right, Time.deltaTime * tireRotationSpeed * currentVelocityOffset);
        }

        EngineSound();
    }

    #endregion

    #region Movement

    private void Movement()
    {
        if(Grounded())
        {
            if(!Input.GetKey(KeyCode.Space))
            {
                Acceleration();
                Rotation();
            }
            Brake();
        }
        else
        {
            Gravity();
        }
        Tilt();
    }

    private void Acceleration()
    {
        sphereRB.velocity = Vector3.Lerp(sphereRB.velocity, 
            //(transform.up * sphereRB.velocity.y) + 
            (maxSpeed * moveInput * transform.forward),
                        //new Vector3(sphereRB.velocity.x, sphereRB.velocity.y, maxSpeed * moveInput), 
                        Time.fixedDeltaTime * acceleration);
    }

    private void Rotation()
    {
        transform.Rotate(0, steerInput * moveInput * currentVelocityOffset * steerStrength * Time.fixedDeltaTime, 0, Space.World);

        //Visuals
        Handle.transform.localRotation = 
            Quaternion.Slerp(Handle.transform.localRotation,
            Quaternion.Euler(Handle.transform.localRotation.eulerAngles.x,
                            handleRotationValue * steerInput,
                            Handle.transform.localRotation.eulerAngles.z), 
            handleRotationSpeed);
    }

    private void Brake()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            sphereRB.velocity *= brakingFactor;
        }
    }

    private void Gravity()
    {
        sphereRB.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }

    private void Tilt()
    {
        float xRot = (Quaternion.FromToRotation(TrikeBody.transform.up, hit.normal) * TrikeBody.transform.rotation).eulerAngles.x;
        float zRot = 0;

        if(currentVelocityOffset > 0)
        {
            zRot = -zTiltAngle * (steerInput<0?steerInput:steerInput/2) * currentVelocityOffset;
        }   

        Quaternion targetRot = Quaternion.Slerp(TrikeBody.transform.rotation, Quaternion.Euler(xRot, transform.eulerAngles.y, zRot), xTiltIncrement);

        Quaternion newRotation = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z);

        TrikeBody.MoveRotation(newRotation);
    }

    #endregion

    #region Trike Checks

    private bool Grounded()
    {
        if(Physics.Raycast(sphereRB.position, Vector3.down, out hit, rayLength, drivable))
        {
            Debug.DrawLine(sphereRB.position, hit.point, Color.red);

            return true;
        }
        else
        {
            Debug.DrawLine(sphereRB.position, sphereRB.position + rayLength * Vector3.down, Color.green);

            return false;
        }
    }

    #endregion

    #region Audio

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentVelocityOffset));
    }

    #endregion
}
