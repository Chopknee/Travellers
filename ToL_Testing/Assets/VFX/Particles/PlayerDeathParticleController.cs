using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class PlayerDeathParticleController : MonoBehaviour
{
    public Light[] lightsInParticles;
    public Color startColor = new Color(16,255,0), endColor = new Color(255,0, 190), currentColor;
    float iterator = 0;
    bool positive;
    public VisualEffect[] fxToPauseOnStart;

    private void Awake()
    {

        lightsInParticles = transform.GetComponentsInChildren<Light>();
        foreach (Light l in lightsInParticles)
            l.enabled = false;
        currentColor = startColor;
        
        foreach(VisualEffect fx in fxToPauseOnStart)
        {
            fx.Stop();
            fx.startSeed = (uint)Random.Range(0, 5);
        }

    }
    



    public void Play(VisualEffect fx)
    {
        if (fx.gameObject.name == "PlayerDeathParticles")
        {
            foreach (Light l in lightsInParticles)
                l.enabled = true;
        }
        fx.enabled = true;
        fx.Play();
    }

    public void Stop(VisualEffect fx)
    {
        if (fx.gameObject.name == "PlayerDeathParticles")
        {
            foreach (Light l in lightsInParticles)
                l.enabled = false;

            fx.Stop();
        }
        else
        {
            fx.gameObject.SetActive(false);
        }

        
    }

    private void Update()
    {
        foreach (Light l in lightsInParticles)
        {
            iterator += (positive) ? 1f : -1f;

            if (iterator <= 0)
            {
                positive = true;
            }
            if(iterator >= 100) positive = false;

            currentColor = (positive) ? currentColor = Color.Lerp(currentColor, endColor, .5f * Time.deltaTime) : currentColor = Color.Lerp(currentColor, startColor, .5f * Time.deltaTime);
            
            l.color = currentColor;
        }
    }
}
