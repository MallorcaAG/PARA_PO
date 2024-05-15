using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    [Header("Vehicle Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;
    [SerializeField] private float brakingDeceleration = 100f;
    [SerializeField] private float brakingDragCoefficient = 0.5f;


    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;


    [Header("Input")]
    [SerializeField] private float moveInput = 0;
    [SerializeField] private float steerInput = 0;

    [Header("Visuals")]
    [SerializeField] private float tireRotSpeed = 3000f;
    [SerializeField] private float maxSteeringAngle = 30f;

    [Header("Audio")]
    [SerializeField]
    [Range(0, 1)] private float minPitch = 1f;
    [SerializeField]
    [Range(1, 5)] private float maxPitch = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody vehicleRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private GameObject[] tires = new GameObject[4];
    [SerializeField] private GameObject[] frontTireParents = new GameObject[2];
    [SerializeField] private AudioSource engineSound;

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    #region Unity Functions
    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        Visuals();
        EngineSound();
    }

    private void Update()
    {
        GetPlayerInput();
    }
    #endregion

    #region Movement

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Acceleration()
    {
        if(currentCarLocalVelocity.z < maxSpeed)
        {
            vehicleRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        }
        //Debug.Log(currentCarLocalVelocity.z);
    }

    private void Deceleration()
    {
        vehicleRB.AddForceAtPosition((Input.GetKey(KeyCode.Space) ? brakingDeceleration : deceleration) * Mathf.Abs(carVelocityRatio) * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        vehicleRB.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio)) * Mathf.Sign(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSidewaysSpeed * (Input.GetKey(KeyCode.Space) ? brakingDragCoefficient : dragCoefficient);

        Vector3 dragForce = transform.right * dragMagnitude;
        vehicleRB.AddForceAtPosition(dragForce, vehicleRB.worldCenterOfMass, ForceMode.Acceleration);
    }

    #endregion

    #region Input Handling

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region Car Status Check

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for(int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelIsGrounded[i];
        }

        if(tempGroundedWheels > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(vehicleRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    #endregion

    #region Suspension Functions
    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;
            float maxDistance = restLength;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(vehicleRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                vehicleRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                //Visuals
                SetTirePosition(tires[i], hit.point + rayPoints[i].up * wheelRadius);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;

                //Visuals
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * maxLength);

                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
        
    }
    #endregion

    #region Visuals

    private void Visuals()
    {
        TireVisuals();
    }

    private void TireVisuals()
    {
        float steeringAngle = maxSteeringAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if(i<2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * carVelocityRatio * Time.deltaTime, Space.Self);

                frontTireParents[i].transform.localEulerAngles = new Vector3(frontTireParents[i].transform.localEulerAngles.x, steeringAngle, frontTireParents[i].transform.localEulerAngles.z);
            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * moveInput * Time.deltaTime, Space.Self);
            }
        }
    }

    private void SetTirePosition(GameObject tire, Vector3 targetPos)
    {
        tire.transform.position = targetPos;
    }

    #endregion

    #region Audio

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocityRatio));
    }

    #endregion
}
