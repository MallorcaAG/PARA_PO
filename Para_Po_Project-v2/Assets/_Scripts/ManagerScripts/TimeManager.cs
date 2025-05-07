using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private GameEvent onDayActivated;
    [SerializeField] private GameEvent onSunsetActivated;
    [SerializeField] private GameEvent onNightActivated;
    [SerializeField] private GameEvent onSunriseActivated;

    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;
    [SerializeField] private Material Skybox_DualPanoramic;

    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;

    [SerializeField] private Light globalLight;

    [Tooltip("Lower means = faster, Higher means = slower")]
    [SerializeField] private float cycleDurationInSeconds = 240f; // Total duration for a full day-night cycle

    private float elapsedTime;
    private float secondsPerHour;
    private int currentHour;
    private float skyboxScrollSpeed;

    // Flag to check if this is the first time the game has started
    private bool isFirstTime = true;

    private void Start()
    {
        // Set initial cycle duration and tick speed
        secondsPerHour = cycleDurationInSeconds / 24f;
        skyboxScrollSpeed = 1f / cycleDurationInSeconds;

        // Always start at noon (12:00 PM)
        if (isFirstTime)
        {
            elapsedTime = secondsPerHour * 12; // Start at noon
            currentHour = 12;

            // Set skybox and light settings for noon/daytime
            RenderSettings.skybox = Skybox_DualPanoramic;
            RenderSettings.skybox.SetTexture("_Texture1", skyboxDay);
            RenderSettings.skybox.SetFloat("_Blend", 0);
            RenderSettings.skybox.SetFloat("_Offset", 0);

            // Reset sun rotation for noon (top of the sky)
            float rotationAngle = (elapsedTime / cycleDurationInSeconds) * 360f;
            globalLight.transform.rotation = Quaternion.Euler(new Vector3(rotationAngle - 90f, 0f, 0f));

            // Set light color and intensity for noon
            globalLight.color = gradientSunriseToDay.Evaluate(1f); // fully into "day"
            globalLight.intensity = 1.0f;
            RenderSettings.sun = globalLight;
            RenderSettings.fogColor = globalLight.color;

            // Fire day event
            onDayActivated.Raise(this, 0f);
            OnHoursChange(currentHour, true);

            // Mark that the game has been initialized
            isFirstTime = false;
        }
    }

    private void Update()
    {
        // Only update elapsed time and hour after the initial setup
        if (!isFirstTime)
        {
            elapsedTime += Time.deltaTime;

            // Update the hour based on elapsed time
            int newHour = Mathf.FloorToInt((elapsedTime / secondsPerHour) % 24);
            if (newHour != currentHour)
            {
                currentHour = newHour;
                OnHoursChange(currentHour, false);
            }

            // Update sun rotation based on elapsed time
            float rotationAngle = (elapsedTime / cycleDurationInSeconds) * 360f;
            globalLight.transform.rotation = Quaternion.Euler(new Vector3(rotationAngle - 90f, 0f, 0f));

            // Update the skybox scroll effect
            if (RenderSettings.skybox != null)
            {
                RenderSettings.skybox.SetFloat("_Offset", Time.time * skyboxScrollSpeed);
            }
            else
            {
                Debug.LogWarning("RenderSettings.skybox is null. Make sure Skybox_DualPanoramic is assigned.");
            }
        }
    }

    private void OnHoursChange(int hour, bool isInitialSetup)
    {
        // Handle transitions for different times of the day
        if (hour == 7) // Sunrise to Day
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
                StartCoroutine(LerpIntensity(globalLight.intensity, 1.0f, secondsPerHour)); // Bright light for daytime
            }
        }
        else if (hour == 17) // Day to Sunset
        {
            onSunsetActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, secondsPerHour));
            StartCoroutine(LerpLight(gradientDayToSunset, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.2f, secondsPerHour)); // Dim light for sunset
        }
        else if (hour == 21) // Sunset to Night
        {
            onNightActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, secondsPerHour));
            StartCoroutine(LerpLight(gradientSunsetToNight, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.05f, secondsPerHour)); // Very dim light for night
        }
        else if (hour == 5) // Night to Sunrise
        {
            onSunriseActivated.Raise(this, 0);
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, secondsPerHour));
            StartCoroutine(LerpLight(gradientNightToSunrise, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.5f, secondsPerHour)); // Dim light for sunrise

            // Transition from sunrise to day
            StartCoroutine(DelayTransitionToDay());
        }
    }

    private IEnumerator DelayTransitionToDay()
    {
        yield return new WaitForSeconds(secondsPerHour);
        StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, secondsPerHour));
        StartCoroutine(LerpLight(gradientSunriseToDay, secondsPerHour));
        StartCoroutine(LerpIntensity(globalLight.intensity, 1.0f, secondsPerHour)); // Transition to bright daytime light
    }

    private IEnumerator LerpSkybox(Texture2D fromTexture, Texture2D toTexture, float duration)
    {
        RenderSettings.skybox.SetTexture("_Texture1", fromTexture);
        RenderSettings.skybox.SetTexture("_Texture2", toTexture);
        RenderSettings.skybox.SetFloat("_Blend", 0);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float blend = t / duration;
            RenderSettings.skybox.SetFloat("_Blend", blend);
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
            float blend = t / duration;
            globalLight.intensity = Mathf.Lerp(fromIntensity, toIntensity, blend);
            yield return null;
        }

        globalLight.intensity = toIntensity;
    }
}
