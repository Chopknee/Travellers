using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractParticles : MonoBehaviour
{
    ParticleSystem pSys;
    public float attractionIntensity = 1f;
    public float attractionSmoothness = .5f;
    public float attractionRadius = 2f;
    public float noise = 2f;

    private void Awake()
    {
        pSys = transform.parent.GetComponent<ParticleSystem>();

    }
    [ExecuteInEditMode]
    private void LateUpdate()
    {
        if (pSys == null) return;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pSys.particleCount + 1];
        int livingParticles = pSys.GetParticles(particles);

        for(int i = 0; i < livingParticles; i++)
        {
            float r = Random.Range(1, noise);
           // particles[i].position = Vector3.Lerp(particles[i].position, transform.position, (attractionIntensity * attractionSmoothness * r) / 5);
            if(Vector3.Distance(particles[i].position, transform.position) < .1f)
            {
                particles[i].remainingLifetime = 1000;
            }
        }
        pSys.SetParticles(particles, livingParticles);
    }
}
