using UnityEngine;
using Cinemachine;

public class FreeLookCameraTuner : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [SerializeField] private float mouseXSensitivity = 200f; // Lower = slower

    [Header("Y Axis Settings (Optional)")]
    [SerializeField] private float yAxisSensitivity = 2f;

    private CinemachineFreeLook freeLookCam;

    void Start()
    {
        freeLookCam = GetComponent<CinemachineFreeLook>();

        if (freeLookCam != null)
        {
            // Set tighter X axis control (mouse)
            freeLookCam.m_XAxis.m_MaxSpeed = mouseXSensitivity;

            // Optional: Set Y axis sensitivity
            freeLookCam.m_YAxis.m_MaxSpeed = yAxisSensitivity;

            // Optional: Prevent overshooting on the Y axis
            freeLookCam.m_YAxis.m_AccelTime = 0.1f;
            freeLookCam.m_YAxis.m_DecelTime = 0.1f;
        }
        else
        {
            Debug.LogWarning("CinemachineFreeLook component not found on this GameObject.");
        }
    }
}
