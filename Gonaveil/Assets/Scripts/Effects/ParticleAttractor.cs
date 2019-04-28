using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour {

    public ParticleSystem particleSystem;

    public float force;

    ParticleSystem.Particle[] particles;

    void Start() {

    }

    // Update is called once per frame
    void LateUpdate() {
        InitializeIfNeeded();

        int numParticlesAlive = particleSystem.GetParticles(particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++) {
            var delta = transform.position - particles[i].position;

            particles[i].velocity += delta.normalized * (1 / delta.magnitude) * force * Time.deltaTime;
        }

        // Apply the particle changes to the Particle System
        particleSystem.SetParticles(particles, numParticlesAlive);
    }

    void InitializeIfNeeded() {
        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
}
