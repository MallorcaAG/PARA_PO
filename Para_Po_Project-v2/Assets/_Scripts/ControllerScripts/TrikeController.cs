using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TrikeController : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 4f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private float zTiltAngle = 45f;
    [SerializeField] private float handleRotationValue = 30f;
    [SerializeField] private float handleRotationSpeed = .15f;
    [SerializeField] private float tireRotationSpeed = 10000f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float xTiltIncrement = 0.1f;
    [SerializeField] private float normalDrag = 2f;
    [SerializeField] private float driftDrag = 0.5f;
    [SerializeField] private AnimationCurve turningCurve;
    [Range(0.6f, 1f)] [SerializeField] private float brakingFactor = 0.9f;

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
    private float currentInput;
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
        ApplyStabilization();
    }

    IEnumerator SendSpeed()
    {
        sendPlayerSpeed.Raise(this, sphereRB.velocity.magnitude.ToString("F4"));
        yield return new WaitForSeconds(1f);
        StartCoroutine(SendSpeed());
    }

    private void Movement()
    {
        if (Grounded())
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                Acceleration();
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

    private void Acceleration()
    {
        currentInput = Mathf.MoveTowards(currentInput, moveInput, Time.fixedDeltaTime * acceleration);
        Vector3 targetVelocity = transform.forward * maxSpeed * currentInput;
        sphereRB.velocity = Vector3.Lerp(sphereRB.velocity, targetVelocity, Time.fixedDeltaTime * acceleration);
    }

    private void Rotation()
    {
        currentSteer = Mathf.MoveTowards(currentSteer, steerInput, Time.fixedDeltaTime * 4f);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(currentVelocityOffset));
        float steerLimit = turningCurve.Evaluate(speedFactor);
        float steerAmount = currentSteer * currentInput * steerLimit * steerStrength;

        transform.Rotate(0, steerAmount * Time.fixedDeltaTime, 0, Space.World);

        // Visual Handle Rotation
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
            sphereRB.velocity *= brakingFactor;
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

        if (currentVelocityOffset > 0)
        {
            zRot = -zTiltAngle * (currentSteer < 0 ? currentSteer : currentSteer / 2) * currentVelocityOffset;
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

        if (Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, rayLength, drivable))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyStabilization()
    {
        // Damp lateral velocity
        Vector3 localVel = transform.InverseTransformDirection(sphereRB.velocity);
        localVel.x = Mathf.Lerp(localVel.x, 0, Time.fixedDeltaTime * 6f); // Strong side grip
        sphereRB.velocity = transform.TransformDirection(localVel);

        // Downforce to help against tipping
        float downforce = Mathf.Abs(currentVelocityOffset) * 50f;
        sphereRB.AddForce(-transform.up * downforce);
    }

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentVelocityOffset));
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
