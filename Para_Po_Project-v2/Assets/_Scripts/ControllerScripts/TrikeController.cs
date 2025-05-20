using System.Collections;
using UnityEngine;

public class TrikeController : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 6f;
    [SerializeField] private float steerStrength = 30f;
    [SerializeField] private float zTiltAngle = 45f;
    [SerializeField] private float handleRotationValue = 30f;
    [SerializeField] private float handleRotationSpeed = 0.15f;
    [SerializeField] private float tireRotationSpeed = 10000f;
    [SerializeField] private float gravity = 30f;
    [SerializeField] private float xTiltIncrement = 5f;
    [SerializeField] private float normalDrag = 3f;
    [SerializeField] private float driftDrag = 1f;
    [SerializeField] private AnimationCurve turningCurve;
    [Range(0.6f, 1f)] [SerializeField] private float brakingFactor = 0.8f;

    [Header("Audio")]
    [SerializeField] [Range(0, 1)] private float minPitch = 1f;
    [SerializeField] [Range(1, 5)] private float maxPitch = 2.5f;

    private float currentInput, currentSteer;
    private float currentVelocityOffset;
    private float rayLength;
    private Vector3 velocity;
    private RaycastHit hit;

    [Header("References")]
    [SerializeField] private Rigidbody sphereRB, TrikeBody;
    [SerializeField] private GameObject Handle;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private GameObject[] tires;

    [Header("Game Events")]
    [SerializeField] private GameEvent sendPlayerSpeed;

    private void Start()
    {
        sphereRB.transform.parent = null;
        TrikeBody.transform.parent = null;
        rayLength = sphereRB.GetComponent<SphereCollider>().radius * 4.83f + 0.2f;

        StartCoroutine(SendSpeed());
    }

    private void Update()
    {
        currentInput = Input.GetAxis("Vertical");
        currentSteer = Input.GetAxis("Horizontal");

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

    private IEnumerator SendSpeed()
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
                SmoothAcceleration();
            else
                Handbrake();

            Rotation();
        }
        else
        {
            ApplyGravity();
        }

        TiltBody();
    }

    private void SmoothAcceleration()
    {
        Vector3 forward = transform.forward;
        float currentSpeed = Vector3.Dot(sphereRB.velocity, forward);
        float targetSpeed = maxSpeed * currentInput;

        float speedDiff = targetSpeed - currentSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > Mathf.Abs(currentSpeed)) ? acceleration : deceleration;

        if (Mathf.Approximately(currentInput, 0f))
        {
            Vector3 decel = -sphereRB.velocity.normalized * deceleration * 0.5f;
            sphereRB.AddForce(decel, ForceMode.Acceleration);
        }
        else
        {
            float force = speedDiff * accelRate;
            sphereRB.AddForce(forward * force, ForceMode.Acceleration);
        }

        // Lateral friction
        Vector3 localVel = transform.InverseTransformDirection(sphereRB.velocity);
        localVel.x = Mathf.Lerp(localVel.x, 0, Time.fixedDeltaTime * 5f);  // Reduce side slipping
        sphereRB.velocity = transform.TransformDirection(localVel);

        sphereRB.drag = normalDrag;
    }

    private void Rotation()
    {
        float turnStrength = turningCurve.Evaluate(Mathf.Abs(currentVelocityOffset));
        transform.Rotate(0, currentSteer * currentInput * turnStrength * steerStrength * Time.fixedDeltaTime, 0, Space.World);

        Handle.transform.localRotation = Quaternion.Slerp(
            Handle.transform.localRotation,
            Quaternion.Euler(Handle.transform.localRotation.eulerAngles.x, handleRotationValue * currentSteer, Handle.transform.localRotation.eulerAngles.z),
            handleRotationSpeed);
    }

    private void Handbrake()
    {
        sphereRB.velocity *= brakingFactor;
        sphereRB.drag = driftDrag;

        Vector3 lateral = Vector3.ProjectOnPlane(sphereRB.velocity, transform.forward);
        sphereRB.AddForce(-lateral * 0.5f, ForceMode.Acceleration);
    }

    private void ApplyGravity()
    {
        sphereRB.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }

    private void TiltBody()
    {
        float xRot = (Quaternion.FromToRotation(TrikeBody.transform.up, hit.normal) * TrikeBody.transform.rotation).eulerAngles.x;
        float zRot = 0f;

        if (currentVelocityOffset > 0)
            zRot = -zTiltAngle * (currentSteer < 0 ? currentSteer : currentSteer / 2) * currentVelocityOffset;

        Quaternion targetRot = Quaternion.Slerp(
            TrikeBody.transform.rotation,
            Quaternion.Euler(xRot, transform.eulerAngles.y, zRot),
            xTiltIncrement);

        Quaternion newRot = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z);
        TrikeBody.MoveRotation(newRot);
    }

    private bool Grounded()
    {
        float radius = rayLength - 0.02f;
        Vector3 origin = sphereRB.transform.position + radius * Vector3.up;

        return Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, rayLength, drivable);
    }

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(currentVelocityOffset));
    }

    private void OnDrawGizmosSelected()
    {
        float radius = rayLength - 0.02f;
        Vector3 origin = sphereRB.transform.position + radius * Vector3.up;

        Gizmos.color = Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, rayLength, drivable)
            ? Color.red
            : Color.green;

        Gizmos.DrawWireSphere(sphereRB.position, radius);
    }
}
