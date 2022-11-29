using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonScript : MonoBehaviour
{

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////// SAMPLING
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    // phi - longitude
    // theta - latitude
    public Vector3 point_on_sphere(float phi, float theta, float r)
    {
        float x = r * Mathf.Sin(theta) * Mathf.Cos(phi);
        float z = r * Mathf.Sin(theta) * Mathf.Sin(phi);
        float y = r * Mathf.Cos(theta); // In Unity Y is up!
        return new Vector3(x, y, z);
    }

    public Vector3 sample_points_randomly_on_sphere(float r_min, float r_max)
    {
        float rand_phi = Random.Range(0.0f, 2.0f * Mathf.PI);
        float rand_theta = Random.Range(0.0f, Mathf.PI);
        float rand_r = Random.Range(r_min, r_max);
        return point_on_sphere(rand_phi, rand_theta, rand_r);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////// TRANSFORM UTILS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Vector3 move_point_on_sphere(Vector3 curr_pos, float delta_long_phi, float delta_lat_theta)
    {
        float x = curr_pos.x;
        float y = curr_pos.y;
        float z = curr_pos.z;
        // Calculate current position.
        float r = Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f) + Mathf.Pow(z, 2.0f)); 
        float theta = Mathf.Acos(y / r); // latitude. In Unity Y is up!
        float phi = Mathf.Atan2(z, x); // longitude. In unity x and z horizontal
        // Update position.
        phi = phi + delta_long_phi;
        theta = theta + delta_lat_theta;
        return point_on_sphere(phi, theta, r);
    }

    public void move_object_on_sphere(Transform transform, float delta_long_phi, float delta_lat_theta)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        // Calculate current position.
        float r = Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f) + Mathf.Pow(z, 2.0f)); 
        float theta = Mathf.Acos(y / r); // latitude. In Unity Y is up!
        float phi = Mathf.Atan2(z, x); // longitude. In unity x and z horizontal
        // Update position.
        phi = phi + delta_long_phi;
        theta = theta + delta_lat_theta;
        transform.position = point_on_sphere(phi, theta, r);
    }

    public void move_on_circle (Transform transform, float r)
    {
        float x  = r * Mathf.Cos(Time.time + Mathf.PI);
        float z  = r * Mathf.Sin(Time.time + Mathf.PI);
        float y = 0.0f;
        transform.position = new Vector3(x,y,z);
    }

    /**
    * string space - "local" or "global"
    */
    public void long_lat_movement(Transform transform, string space, float delta_lat_theta, float delta_long_phi)
    {
        // https://math.stackexchange.com/questions/268064/move-a-point-up-and-down-along-a-sphere
        // NOTE that in Unity Y is up!
        float x, y, z;
        if (space == "local")
        {
            // If the transform has no parent, it is the same as Transform.position.
            // https://docs.unity3d.com/ScriptReference/Transform-localPosition.html
            x = transform.localPosition.x;
            z = transform.localPosition.z;
            y = transform.localPosition.y;
        }
        else
        {
            x = transform.position.x;
            z = transform.position.z;
            y = transform.position.y;
        }
        // Calculate current position.
        float r = Mathf.Sqrt(Mathf.Pow(x, 2.0f) + Mathf.Pow(y, 2.0f) + Mathf.Pow(z, 2.0f)); 
        float theta = Mathf.Acos(y / r); // latitude. In Unity Y is up!
        float phi = Mathf.Atan2(z, x); // longitude. In unity x and z horizontal
        // Update position.
        phi = phi + delta_long_phi;
        theta = theta + delta_lat_theta;
        if (space == "local")
        {
            transform.localPosition = point_on_sphere(phi, theta, r);
        }
        else
        {
            transform.position = point_on_sphere(phi, theta, r);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////// PARTICLE SYSTEM UTILS
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void create_ps (GameObject game_object)
    {
        // Add ParticleSytem to the given GameObject.
        game_object.AddComponent<ParticleSystem>();        
    }

    void add_ps_material (ParticleSystem ps)
    {
        // Set material to particle system.
        // https://stackoverflow.com/questions/47677764/change-particle-system-material-unity-3d-script
        // https://docs.unity3d.com/ScriptReference/Resources.Load.html
        var ps_renderer = ps.GetComponent<ParticleSystemRenderer>(); // http://answers.unity.com/comments/1551224/view.html, https://stackoverflow.com/questions/50236252/particle-system-pink-when-attached-to-a-gameobject
        ps_renderer.material = Resources.Load<Material>("ParticleMaterial/ParticleMaterial");
    }

    void set_ps_particle_velocity (ParticleSystem ps, Vector3 velocity_dir, float velocity_amount)
    {
        // Get Particles.
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        ps.GetParticles(particles);

        // Set velocity for all particles.
        for (int i = 0; i < particles.Length; ++i)
        {
            // Get particle.
            ParticleSystem.Particle p = particles[i];

            // Set velocity.
            p.velocity += velocity_dir * velocity_amount;

            // Set particle.
            particles[i] = p;
        }

        // Set particles.
        ps.SetParticles(particles, particles.Length);
    }

    void ini_ps_particles(ParticleSystem ps, int n_particles)
    {
        ps.Clear();
        ParticleSystem.Particle[] ps_particles = new ParticleSystem.Particle[n_particles];
        ps.GetParticles(ps_particles);
        for (int i = 0; i < n_particles; ++i)
        {
            Debug.Log(ps_particles[i].position);
            //ps_particles[i].position = common_script.sample_points_randomly_on_sphere(5.0f, 10.0f);;
            //ps_particles[i].velocity = Vector3.zero;

            ParticleSystem.Particle particle_i = ps_particles[i];
            particle_i.velocity = Vector3.zero;
            particle_i.position = sample_points_randomly_on_sphere(50.0f, 100.0f);
            //particle_i.size = 5.0f;
            particle_i.remainingLifetime = 100.0f;
            ps_particles[i] = particle_i;
            Debug.Log(ps_particles[i].position);
        }
        // Apply the particles change to the particle system.
        ps.SetParticles(ps_particles, n_particles);
        ps.Emit(ps_particles.Length);
    }

    // https://www.youtube.com/watch?v=KsT_ZyTv1ms
    // https://docs.unity3d.com/ScriptReference/ParticleSystem.GetParticles.html
    // https://answers.unity.com/questions/1034782/manually-placing-particles.html?childToView=1573665#answer-1573665
    // https://answers.unity.com/questions/260933/how-do-you-set-particles-to-known-positions.html
    // Particles never die.
    void set_ps_position_velocity(ParticleSystem ps, Vector3 position, Vector3 velocity, int n_particles)
    {
        // Get all particles.
        ParticleSystem.Particle[] ps_particles = new ParticleSystem.Particle[n_particles];
        ps.GetParticles(ps_particles);
        for (int i = 0; i < n_particles; ++i)
        {
            ps_particles[i].position = position;
            ps_particles[i].velocity = velocity;

            //ParticleSystem.Particle particle_i = ps_particles[i];
            //particle_i.velocity = Vector3.zero;
            //particle_i.position = common_script.sample_points_randomly_on_sphere(5.0f, 10.0f);
            //ps_particles[i] = particle_i;
        }
        
        // Apply the particles change to the particle system.
        ps.SetParticles(ps_particles, n_particles);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////// RAYCASTING
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Generate ray from screen point where crosshair is using camera.
    private struct RaycastHitInfo
    {
        bool hit;
        Vector3 hit_point; 
        // TODO: object which is hit.
    }
    public void raycast_from_crosshair(Camera camera, out Ray raycast_ray, out bool is_hit, out RaycastHit hit_context)
    {
        Vector3 crosshair_pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        raycast_ray = camera.ScreenPointToRay(crosshair_pos);
        is_hit = false;
        if (Physics.Raycast(raycast_ray, out hit_context, Mathf.Infinity, Physics.DefaultRaycastLayers))
        {
            is_hit = true;
        }
    }

    // https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
    // https://docs.unity3d.com/ScriptReference/RaycastHit.html
    // https://docs.unity3d.com/ScriptReference/Ray.html
    public Vector3 raycast_on_pointer(Camera camera, float fallback_dist)
    {
        RaycastHit hit; 
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers))
        {
            return hit.point; //ray.origin + hit.distance * ray.direction;
        }
        else
        {
            return ray.origin + fallback_dist * ray.direction;
        }
    }

    public void object_to_pointer_in_scene(Camera camera, float distance_from_camera, GameObject game_object)
    {
        // Mouse position in 3D scene.
        // https://github.com/MirzaBeig/FireflySparkleMouseFollowTutorial/blob/master/FollowMouse.cs
        Vector3 mouse_position = Input.mousePosition;
        mouse_position.z = distance_from_camera;
        game_object.transform.position = camera.ScreenToWorldPoint(mouse_position);
    }

    //
    // THROW
    //

    // Generate ray from crosshair using camera.
    // Assume that game_object has rigid body component.
    public void throw_from_camera(Camera camera, GameObject game_object)
    {
        Vector3 crosshair_pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = camera.ScreenPointToRay(crosshair_pos);
        Rigidbody rb = game_object.GetComponent<Rigidbody>();
        rb.AddForce(ray.direction * 60.0f);
    }

}

