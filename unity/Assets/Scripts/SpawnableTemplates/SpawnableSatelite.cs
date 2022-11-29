using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableSatelite : MonoBehaviour
{
    // Variables set by creator script (Player.cs).
    [HideInInspector] 
    public Ray spawn_ray; // ray which is used for spawning and moving the planet.
    [HideInInspector] 
    public float movement_speed = 100.0f; // speed used for trowing
    [HideInInspector] 
    public float max_dist = 100000.0f; // max distance from planet

    // Reference to script components.
    private CommonScript common_script;

    // Private variables.
    private Vector3 planet_center;
    private Vector3 starting_position;
    private Vector3 movement_direction;
    private float starting_distance;
    private bool started_circling;
    private float t_prox;
    private float circle_speed;

    // Start is called before the first frame update
    void Start()
    {
        // Resusable/common code.
        common_script = this.GetComponent<CommonScript>();

        // Set variables given by creator script.
        starting_position = spawn_ray.origin;
        movement_direction = spawn_ray.direction;
        starting_distance = starting_position.magnitude;
        started_circling = false;
        t_prox = Random.Range(0.1f, 0.3f);
        circle_speed = Random.Range(5.0f, 10.0f);

        // Creator script set satelite as child to planet (parent).
        planet_center = gameObject.transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the planet should move towards center or start circling.
        float curr_dist_from_center = (transform.position - planet_center).magnitude;
        float t = curr_dist_from_center / starting_distance;
        if (started_circling || t < t_prox)
        {
            // TODO: if direction is away from center, tract distance as well!
            // TODO: rotate circle on which rotation is done.
            started_circling = true;
            transform.localPosition = common_script.move_point_on_sphere(transform.localPosition, Time.deltaTime * circle_speed, 0.0f);
            // Rotate.
            transform.Rotate(0, 0, 120 * Time.deltaTime);
        }
        else if (!started_circling)
        {
            // Move planet in spawn ray direction.
            transform.position += movement_direction * movement_speed;
        }

        // Destroy if object is too far from the circle.
        if (curr_dist_from_center > max_dist)
        {
            // Too far, destroy.
            Destroy(gameObject, 10.0f);
        }
    }
}
