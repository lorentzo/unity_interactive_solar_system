using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCenter : MonoBehaviour
{
    // public script variables.
    public float sun_circle_radius = 200.0f;
    public int n_sun_circle_points = 200;
    [SerializeField] Material sun_circle_material; // https://stackoverflow.com/questions/60389773/get-material-from-assets

    // Reference to script components.
    private DrawLine draw_line_script;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch scripts.
        draw_line_script = this.GetComponent<DrawLine>();

        // Draw circle.
        draw_line_script.draw_circle(gameObject, sun_circle_radius, n_sun_circle_points, sun_circle_material);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
