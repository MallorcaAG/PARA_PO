using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Texture2D skyboxNight;
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;

    [SerializeField] private Gradient gradientNightToSunrise;
    [SerializeField] private Gradient gradientSunriseToDay;
    [SerializeField] private Gradient gradientDayToSunset;
    [SerializeField] private Gradient gradientSunsetToNight;

    [SerializeField] private Light globalLight;

    [SerializeField] private float cycleDurationInSeconds = 240f; // Total duration for a full day-night cycle

    private float elapsedTime;
    private float secondsPerHour;
    private int currentHour;
    private float skyboxScrollSpeed;

    private void Start()
    {
        secondsPerHour = cycleDurationInSeconds / 24f; // Divide total duration by 24 hours
        skyboxScrollSpeed = 1f / cycleDurationInSeconds; // Scroll speed proportional to cycle duration
        elapsedTime = secondsPerHour * 8; // Start at hour 8, which is daytime
        currentHour = 8;
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

        // Calculate the sun's rotation angle based on the elapsed time and cycle speed
        float rotationAngle = (elapsedTime / cycleDurationInSeconds) * 360f;
        globalLight.transform.rotation = Quaternion.Euler(new Vector3(rotationAngle - 90f, 0f, 0f));

        // Scroll the skybox texture to simulate moving clouds
        RenderSettings.skybox.SetFloat("_Offset", Time.time * skyboxScrollSpeed);
    }

    private void OnHoursChange(int hour, bool isInitialSetup)
    {
        if (hour == 8)
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
                StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, secondsPerHour));
                StartCoroutine(LerpLight(gradientSunriseToDay, secondsPerHour));
                StartCoroutine(LerpIntensity(globalLight.intensity, 1.0f, secondsPerHour)); // Bright light for daytime
            }
        }
        else if (hour == 18)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, secondsPerHour));
            StartCoroutine(LerpLight(gradientDayToSunset, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.7f, secondsPerHour)); // Dimmer light for sunset
        }
        else if (hour == 22)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, secondsPerHour));
            StartCoroutine(LerpLight(gradientSunsetToNight, secondsPerHour));
            StartCoroutine(LerpIntensity(globalLight.intensity, 0.3f, secondsPerHour)); // Very dim light for night
        }
        else if (hour == 6)
        {
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
