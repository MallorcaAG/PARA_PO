using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvent onDayActivated;
    [SerializeField] private GameEvent onSunsetActivated;
    [SerializeField] private GameEvent onNightActivated;
    [SerializeField] private GameEvent onSunriseActivated;

    [Header("Skybox Textures")]
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;
    [SerializeField] private Material Skybox_DualPanoramic;

    [Header("Lighting")]
    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;
    [SerializeField] private Light globalLight;

    [Tooltip("Lower = faster day cycle")]
    [SerializeField] private float cycleDurationInSeconds = 240f;

    private float elapsedTime;
    private float secondsPerHour;
    private int currentHour;
    private float skyboxScrollSpeed;
    private Material skyboxInstance;

    private void Start()
    {
        // Always reset time to noon
        secondsPerHour = cycleDurationInSeconds / 24f;
        elapsedTime = secondsPerHour * 12f; // Start at noon
        currentHour = 12;
        skyboxScrollSpeed = 1f / cycleDurationInSeconds;

        // Instantiate a new skybox material to prevent cross-scene blending issues
        skyboxInstance = new Material(Skybox_DualPanoramic);
        RenderSettings.skybox = skyboxInstance;
        RenderSettings.skybox.SetTexture("_Texture1", skyboxDay);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        RenderSettings.skybox.SetFloat("_Offset", 0);

        // Set light rotation, color, and intensity to noon
        float rotationAngle = (elapsedTime / cycleDurationInSeconds) * 360f;
        globalLight.transform.rotation = Quaternion.Euler(new Vector3(rotationAngle - 90f, 0f, 0f));
        globalLight.color = gradientSunriseToDay.Evaluate(1f);
        globalLight.intensity = 1.0f;
        RenderSettings.sun = globalLight;
        RenderSettings.fogColor = globalLight.color;

        // Trigger day events
        onDayActivated.Raise(this, 0f);
        OnHoursChange(currentHour, true);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        int newHour = Mathf.FloorToInt((elapsedTime / secondsPerHour) % 24);
        if (newHour != currentHour)
        {
            currentHour = newHour;
            OnHoursChange(currentHour, false);
        }

        float rotationAngle = (elapsedTime / cycleDurationInSeconds) * 360f;
        globalLight.transform.rotation = Quaternion.Euler(new Vector3(rotationAngle - 90f, 0f, 0f));

        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Offset", Time.time * skyboxScrollSpeed);
        }
    }

    private void OnHoursChange(int hour, bool isInitialSetup)
    {
        if (hour == 7)
        {
            if (isInitialSetup)
            {
                RenderSettings.skybox.SetTexture("_Texture1", skyboxDay);
                RenderSettings.skybox.SetFloat("_Blend", 0);
                globalLight.color = gradientSunriseToDay.Evaluate(1f);
                globalLight.intensity = 1.0f;
                RenderSettings.fogColor = globalLight.color;
            }
            else
            {
                onDayActivated.Raise(this, 0);
                StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, secondsPerHour));
                StartCoroutine(LerpLight(gradientSunriseToDay, secondsPerHour));
                StartCoroutine(LerpIntensity(globalLight.intensity, 1.0f, secondsPerHour));
            }
        }
        else if (hour == 17)
        {
            onSunsetActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, secondsPerHour));
            StartCoroutine(LerpLight(gradientDayToSunset, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.2f, secondsPerHour));
        }
        else if (hour == 21)
        {
            onNightActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, secondsPerHour));
            StartCoroutine(LerpLight(gradientSunsetToNight, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.05f, secondsPerHour));
        }
        else if (hour == 5)
        {
            onSunriseActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, secondsPerHour));
            StartCoroutine(LerpLight(gradientNightToSunrise, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.5f, secondsPerHour));
            StartCoroutine(DelayTransitionToDay());
        }
    }

    private IEnumerator DelayTransitionToDay()
    {
        yield return new WaitForSeconds(secondsPerHour);
        StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, secondsPerHour));
        StartCoroutine(LerpLight(gradientSunriseToDay, secondsPerHour));
        StartCoroutine(LerpIntensity(globalLight.intensity, 1.0f, secondsPerHour));
    }

    private IEnumerator LerpSkybox(Texture2D fromTexture, Texture2D toTexture, float duration)
    {
        RenderSettings.skybox.SetTexture("_Texture1", fromTexture);
        RenderSettings.skybox.SetTexture("_Texture2", toTexture);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", t / duration);
            yield return null;
        }

        RenderSettings.skybox.SetTexture("_Texture1", toTexture);
        RenderSettings.skybox.SetFloat("_Blend", 0);
    }

    private IEnumerator LerpLight(Gradient lightGradient, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float progress = t / duration;
            globalLight.color = lightGradient.Evaluate(progress);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }

    private IEnumerator LerpIntensity(float fromIntensity, float toIntensity, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            globalLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, t / duration);
            yield return null;
        }
        globalLight.intensity = toIntensity;
    }
}
