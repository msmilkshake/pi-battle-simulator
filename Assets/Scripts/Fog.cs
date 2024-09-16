using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public GameObject fog;
    public ParticleSystem fogParticleSystem;
    public Boolean isActive;
    public float environmentLightMultiplierFog = 0.3f;
    public float environmentLightMultiplierNormal = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        if (fogParticleSystem != null)
        {
            fogParticleSystem.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isActive)
            {
                ActivateFog();
            }
            else
            {
                DeactivateFog();
            }
        }   
    }
    
    void ActivateFog()
    {
        // Start the fog particle system
        if (fogParticleSystem != null)
        {
            fog.SetActive(true);
            fogParticleSystem.Play();
            isActive = true;
        }

        // Adjust the Environment Lighting multiplier
        RenderSettings.ambientIntensity = environmentLightMultiplierFog;
    }
    
    void DeactivateFog()
    {
        // Start the fog particle system
        if (fogParticleSystem != null)
        {
            fog.SetActive(false);
            fogParticleSystem.Stop();
            isActive = false;
        }

        // Adjust the Environment Lighting multiplier
        RenderSettings.ambientIntensity = environmentLightMultiplierNormal;
    }
    
    
}
