using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSeek : MonoBehaviour
{
    public Transform target;
    public float force = 10.0f;

    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // LateUpdate.
    // We want to PS simulation to be done after we apply our forces.
    void LateUpdate()
    {
        // Get Particles.
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);

        // Set velocity for all particles.
        for (int i = 0; i < particles.Length; ++i)
        {
            // Get particle.
            ParticleSystem.Particle p = particles[i];

            // Caclulate velocity.
            Vector3 direction_to_target = (target.position - p.position).normalized;
            Vector3 seek_force = direction_to_target * force * Time.deltaTime;
            p.velocity += seek_force;// Vector3.Scale(p.velocity, seek_force);

            // Set particle.
            particles[i] = p;
        }

        // Set particles.
        ps.SetParticles(particles, particles.Length);
    }
}
