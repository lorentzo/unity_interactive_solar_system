using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnablePlanet : MonoBehaviour
{
    // Variables set by creator script (Player.cs).
    [HideInInspector] 
    public Ray spawn_ray; // ray which is used for spawning and moving the planet.
    [HideInInspector] 
    public float movement_speed; // speed used for trowing
    [HideInInspector] 
    public float max_dist; // max distance from circling center
    [HideInInspector] 
    public float max_scale; // max distance from circling center

    // Reference to game object components.
    private CommonScript common_script;
    private TrailRenderer trail_renderer;

    // Private variables.
    private Vector3 sun_position;
    private Vector3 starting_position;
    private Vector3 movement_direction;
    private float starting_distance;
    private bool started_circling;
    float t_prox;
    private Vector3 curr_scale;
    private float rotate_dir;
    private float rotate_speed;
    private float circling_speed;
    private float scaling_speed;


    // Start is called before the first frame update
    void Start()
    {
        // Resusable/common code.
        common_script = this.GetComponent<CommonScript>();
        trail_renderer = this.GetComponent<TrailRenderer>();

        // Set variables.
        starting_position = spawn_ray.origin;
        movement_direction = spawn_ray.direction;
        starting_distance = starting_position.magnitude;
        sun_position = gameObject.transform.parent.position; // creator script set planet (a child) to sun (parent)
        started_circling = false;
        t_prox = Random.Range(0.2f, 0.8f);
        curr_scale = gameObject.transform.localScale;
        rotate_dir = 1.0f;
        if (Random.Range(-10.0f, 10.0f) > 5.0f)
        {
            rotate_dir = -1.0f;
        }
        rotate_speed = Random.Range(30.0f, 50.0f);
        circling_speed = Random.Range(0.1f, 0.2f);
        scaling_speed = Random.Range(0.5f, 1.0f);

        // Trail renderer parameters.
        trail_renderer.emitting = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.Log("Entering sun circle!");
            started_circling = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        string game_object_name = collider.gameObject.name.ToLower();
        if (game_object_name.Contains("sun"))
        {
            Debug.Log("Entering sun circle of: " + collider.gameObject.name);
            started_circling = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (started_circling)
        {
            transform.localPosition = common_script.move_point_on_sphere(transform.localPosition, -rotate_dir * Time.deltaTime * circling_speed, 0.0f);
            // Rotate.
            transform.Rotate(0, rotate_dir * rotate_speed * Time.deltaTime, 0);
            // Grow.
            if (transform.localScale.x < max_scale)
            {
                var parent = gameObject.transform.parent;
                gameObject.transform.parent = null; // https://stackoverflow.com/a/64325915
                float scale_change = Time.deltaTime * scaling_speed;
                curr_scale += new Vector3(scale_change, scale_change, scale_change);
                transform.localScale = curr_scale;
                if (transform.localScale.x > max_scale / 2.0f)
                {
                    trail_renderer.emitting = true;
                }
                gameObject.transform.parent = parent;
            }
        }
        else
        {
            transform.position += movement_direction * movement_speed;
        }
        float curr_dist_from_center = (transform.position - sun_position).magnitude;
        /*
        // Check if the planet should move towards sun or start circling.
        
        float t = curr_dist_from_center / starting_distance;
        if (started_circling || t < t_prox)
        {
            // TODO: if direction is away from center, tract distance as well!
            // TODO: rotate circle on which rotation is done.
            transform.localPosition = common_script.move_point_on_sphere(transform.localPosition, Time.deltaTime * 1.0f, 0.0f);
            // Rotate.
            transform.Rotate(0, 0, 120 * Time.deltaTime);
        }
        
        else if (!started_circling)
        {
            // Move planet in spawn ray direction.
            transform.position += movement_direction * movement_speed;
        }
        */
        // Destroy if object is too far from the circle.
        if (curr_dist_from_center > max_dist)
        {
            // Too far, destroy.
            Destroy(gameObject, 10.0f);
        }
    }
}
