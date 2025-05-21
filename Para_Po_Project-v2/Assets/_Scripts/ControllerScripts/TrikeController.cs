using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TrikeController : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float maxSpeed = 20f; // in m/s (72 km/h)
    [SerializeField] private float accelerationRate = 5f; // m/s²
    [SerializeField] private float decelerationRate = 8f; // m/s² when releasing input
    [SerializeField] private float steerStrength = 20f;
    [SerializeField] private float zTiltAngle = 45f;
    [SerializeField] private float handleRotationValue = 30f;
    [SerializeField] private float handleRotationSpeed = 0.15f;
    [SerializeField] private float tireRotationSpeed = 10000f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float xTiltIncrement = 0.1f;
    [SerializeField] private float normalDrag = 2f;
    [SerializeField] private float driftDrag = 0.5f;
    [SerializeField] private AnimationCurve turningCurve;
    [Range(0.6f, 1f)] [SerializeField] private float brakingFactor = 0.8f;

    [Header("Audio")]
    [SerializeField] [Range(0, 1)] private float minPitch = 1f;
    [SerializeField] [Range(1, 5)] private float maxPitch = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody sphereRB, TrikeBody;
    [SerializeField] private GameObject Handle;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private GameObject[] tires;

    [Header("Game Events")]
    [SerializeField] private GameEvent sendPlayerSpeed;

    private float moveInput, steerInput;
    private float rayLength;
    private float currentVelocityOffset;
    private Vector3 velocity;
    private RaycastHit hit;
    private float currentSpeed = 0f;
    private float currentSteer;

    private void Start()
    {
        sphereRB.transform.parent = null;
        TrikeBody.transform.parent = null;
        rayLength = sphereRB.GetComponent<SphereCollider>().radius * 4.83f + 0.2f;
        StartCoroutine(SendSpeed());
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Vertical"); // W/S or Up/Down arrows
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
        ApplyStabilization();
    }

    IEnumerator SendSpeed()
    {
        while (true)
        {
            float speedKPH = sphereRB.velocity.magnitude * 3.6f;
            sendPlayerSpeed.Raise(this, speedKPH.ToString("F0")); 
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Movement()
    {
        if (Grounded())
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                Accelerate();
            }

            Rotation();
            Brake();
        }
        else
        {
            Gravity();
        }

        Tilt();
    }

    private void Accelerate()
    {
        float targetSpeed = maxSpeed * moveInput;

        if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed))
        {
            
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        }
        else
        {
           
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, decelerationRate * Time.fixedDeltaTime);
        }

        Vector3 newVelocity = transform.forward * currentSpeed;
        sphereRB.velocity = new Vector3(newVelocity.x, sphereRB.velocity.y, newVelocity.z);
    }

    private void Rotation()
    {
        currentSteer = Mathf.MoveTowards(currentSteer, steerInput, Time.fixedDeltaTime * 6f);

        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentVelocityOffset));
        float steerLimit = turningCurve.Evaluate(speedFactor);

       
        float directionSign = Mathf.Sign(Vector3.Dot(sphereRB.velocity, transform.forward));
        float steerAmount = currentSteer * steerLimit * steerStrength * directionSign;

        transform.Rotate(0, steerAmount * Time.fixedDeltaTime, 0, Space.World);

        
        Handle.transform.localRotation = Quaternion.Slerp(
            Handle.transform.localRotation,
            Quaternion.Euler(Handle.transform.localRotation.eulerAngles.x,
                             handleRotationValue * currentSteer,
                             Handle.transform.localRotation.eulerAngles.z),
            handleRotationSpeed);
    }

    private void Brake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed *= brakingFactor;
            sphereRB.drag = driftDrag;
        }
        else
        {
            sphereRB.drag = normalDrag;
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

        if (currentSpeed > 0.1f)
        {
            zRot = -zTiltAngle * (currentSteer < 0 ? currentSteer : currentSteer / 2) * (currentSpeed / maxSpeed);
        }

        Quaternion targetRot = Quaternion.Slerp(
            TrikeBody.transform.rotation,
            Quaternion.Euler(xRot, transform.eulerAngles.y, zRot),
            xTiltIncrement);

        Quaternion newRotation = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z);
        TrikeBody.MoveRotation(newRotation);
    }

    private bool Grounded()
    {
        float radius = rayLength - 0.02f;
        Vector3 origin = sphereRB.transform.position + radius * Vector3.up;

        return Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, rayLength, drivable);
    }

    private void ApplyStabilization()
    {
        Vector3 localVel = transform.InverseTransformDirection(sphereRB.velocity);
        localVel.x = Mathf.Lerp(localVel.x, 0, Time.fixedDeltaTime * 6f);
        sphereRB.velocity = transform.TransformDirection(localVel);

        float downforce = Mathf.Abs(currentSpeed / maxSpeed) * 50f;
        sphereRB.AddForce(-transform.up * downforce);
    }

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentSpeed / maxSpeed));
    }

    private void OnDrawGizmosSelected()
    {
        float radius = rayLength - 0.02f;
        Vector3 origin = sphereRB.transform.position + radius * Vector3.up;

        if (Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, rayLength, drivable))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(sphereRB.position, radius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(sphereRB.position, radius);
        }
    }
}
