using System.Collections;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness = 35000f;
    [SerializeField] private float damperStiffness = 4500f;
    [SerializeField] private float restLength = 0.4f;
    [SerializeField] private float springTravel = 0.3f;
    [SerializeField] private float wheelRadius = 0.35f;

    [Header("Vehicle Settings")]
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float maxSpeed = 70f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float brakingDeceleration = 30f;
    [SerializeField] private float steerStrength = 8f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1.5f;
    [SerializeField] private float brakingDragCoefficient = 0.8f;
    [SerializeField] private float gravity = 9.81f;

    private Rigidbody vehicleRB;

    private Vector3 localVelocity;
    private float velocityRatio;

    [Header("Input")]
    private float moveInput;
    private float steerInput;

    [Header("Visuals")]
    [SerializeField] private GameObject[] tires = new GameObject[4];
    [SerializeField] private GameObject[] frontTireParents = new GameObject[2];
    [SerializeField] private float tireRotSpeed = 3000f;
    [SerializeField] private float maxSteeringAngle = 25f;
    [Tooltip("For the km/h visual.\nGet irl distance and divide by seconds taken in-game to travel that same distance. Then divide that with the initial value of km/h on visual. Thats your scale factor" +
        "\n\nfactor * initialKmph = irlDist / timeInGame")]
    [SerializeField] private float scaleFactor = 1f;

    [Header("References")]
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private AudioSource engineSound;

    [Header("Audio")]
    [Range(0, 1)] [SerializeField] private float minPitch = 1f;
    [Range(1, 5)] [SerializeField] private float maxPitch = 3f;

    [Header("Game Events")]
    [SerializeField] private GameEvent sendPlayerSpeed;

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded;

    private float currentForwardSpeed = 0f;

    #region Unity Callbacks

    private void Start()
    {
        vehicleRB = GetComponent<Rigidbody>();
        StartCoroutine(SendSpeedCoroutine());
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateLocalVelocity();

        if (isGrounded)
        {
            HandleMovement();
        }
        else
        {
            ApplyGravity();
        }

        UpdateVisuals();
        UpdateEngineSound();
    }

    #endregion

    #region Movement Methods

    private void HandleMovement()
    {
        ApplyAccelerationAndDeceleration();
        ApplyTurning();
        ApplySidewaysDrag();
    }

    private void ApplyAccelerationAndDeceleration()
    {
        bool braking = Input.GetKey(KeyCode.Space);

        if (braking)
        {
            currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, 0, brakingDeceleration * Time.fixedDeltaTime);
        }
        else if (moveInput > 0)
        {
            float targetSpeed = maxSpeed * moveInput;
            currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else if (moveInput < 0)
        {
            currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, -maxSpeed * 0.3f, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        Vector3 force = currentForwardSpeed * transform.forward;
        vehicleRB.AddForceAtPosition(force, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void ApplyTurning()
    {
        float steerFactor = turningCurve.Evaluate(Mathf.Abs(velocityRatio)) * Mathf.Sign(velocityRatio);
        Vector3 torque = steerStrength * steerInput * steerFactor * transform.up;
        vehicleRB.AddTorque(torque, ForceMode.Acceleration);
    }

    private void ApplySidewaysDrag()
    {
        float dragFactor = Input.GetKey(KeyCode.Space) ? brakingDragCoefficient : dragCoefficient;
        float sidewaysSpeed = localVelocity.x;
        Vector3 dragForce = -sidewaysSpeed * dragFactor * transform.right;
        vehicleRB.AddForceAtPosition(dragForce, vehicleRB.worldCenterOfMass, ForceMode.Acceleration);
    }

    private void ApplyGravity()
    {
        vehicleRB.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    #endregion

    #region Suspension

    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(vehicleRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float damperForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - damperForce;

                vehicleRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                SetTirePosition(tires[i], hit.point + rayPoints[i].up * wheelRadius);
                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;
                SetTirePosition(tires[i], rayPoints[i].position - rayPoints[i].up * maxLength);
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position - rayPoints[i].up * (maxLength + wheelRadius), Color.green);
            }
        }
    }

    #endregion

    #region Input

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    #endregion

    #region Utility

    private void GroundCheck()
    {
        int groundedWheels = 0;
        for (int i = 0; i < wheelIsGrounded.Length; i++)
            groundedWheels += wheelIsGrounded[i];

        isGrounded = groundedWheels > 1;
    }

    private void CalculateLocalVelocity()
    {
        localVelocity = transform.InverseTransformDirection(vehicleRB.velocity);
        velocityRatio = localVelocity.z / maxSpeed;
    }

    #endregion

    #region Visuals

    private void UpdateVisuals()
    {
        UpdateTireRotation();
    }

    private void UpdateTireRotation()
    {
        float steeringAngle = maxSteeringAngle * steerInput;

        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * velocityRatio * Time.deltaTime, Space.Self);

                Vector3 currentAngles = frontTireParents[i].transform.localEulerAngles;
                frontTireParents[i].transform.localEulerAngles = new Vector3(currentAngles.x, steeringAngle, currentAngles.z);
            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * currentForwardSpeed / maxSpeed * Time.deltaTime, Space.Self);
            }
        }
    }

    private void SetTirePosition(GameObject tire, Vector3 position)
    {
        tire.transform.position = position;
    }

    #endregion

    #region Audio

    private void UpdateEngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(velocityRatio));
    }

    #endregion

    #region Coroutine

    private IEnumerator SendSpeedCoroutine()
    {
        while (true)
        {
            sendPlayerSpeed.Raise(this, (vehicleRB.velocity.magnitude * scaleFactor).ToString("F4"));
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}

