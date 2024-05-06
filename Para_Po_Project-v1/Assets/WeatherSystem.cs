using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [SerializeField] private Material defaultSkybox;
    [SerializeField] private Material rainSkybox;
    [SerializeField] private GameObject rainParticles;


    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = defaultSkybox;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R))
        {
            if (RenderSettings.skybox == defaultSkybox) 
            {
                RenderSettings.skybox = rainSkybox;
                rainParticles.SetActive(true);
            }
            else
            {
                RenderSettings.skybox = defaultSkybox;
                rainParticles.SetActive(false);
            }
        }
    }
}
