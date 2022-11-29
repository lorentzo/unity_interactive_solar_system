using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // References to components attached to this game object.
    private CommonScript common_script;
    private Camera this_camera;

    // Script public variables.
    public GameObject[] spawnable_sun_templates;
    public GameObject[] spawnable_planet_templates;
    public GameObject[] spawnable_satelite_templates;

    // Script private variables.
    private int n_spawnable_planets;
    private int n_spawnable_satelites;
    private int n_spawnable_suns;
    
    // Camera movement parameters.
    public float cam_sensitivity = 150.0f;
    public float cam_speed = 50.0f;
    public float cam_speed_fast_fact = 3.0f;
    private float rot_x = 0.0f;
    private float rot_y = 0.0f;
    private bool sphere_walking_mode;

    void Awake()
    {
        // Resusable/common code.
        common_script = this.GetComponent<CommonScript>();

        // Get the camera on which this script is attached to.
        this_camera = this.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Cursor is placed in the center of the view and cannot be moved.
        // https://docs.unity3d.com/ScriptReference/Cursor-lockState.html
        Cursor.lockState = CursorLockMode.Locked;

        // Movement params.
        sphere_walking_mode = false;

        // Prepare spawnable information.
        n_spawnable_planets = spawnable_planet_templates.Length;
        n_spawnable_satelites = spawnable_satelite_templates.Length;
        n_spawnable_suns = spawnable_sun_templates.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation based on mouse input.
        // Translation based on keyboard input.
        if (sphere_walking_mode)
        {
            sphere_movement();
        }
        else
        {
            free_flying_movement();
        }

        if (Input.GetKeyDown("q"))
        {
            Ray raycast_ray;
            bool is_hit = false;
            RaycastHit hit_context;
            common_script.raycast_from_crosshair(this_camera, out raycast_ray, out is_hit, out hit_context);
            if (is_hit)
            {
                string hit_gameobject_name = hit_context.collider.gameObject.name.ToLower();
                if (hit_gameobject_name.Contains("planet"))
                {
                    sphere_walking_mode = true;
                    gameObject.transform.parent = hit_context.collider.gameObject.transform;
                }
            }
        }

        if (Input.GetKeyDown("e"))
        {
            sphere_walking_mode = false;
            gameObject.transform.parent = null;
        }

        // On mouse click...
        if (Input.GetMouseButtonDown(0))
        {
            Ray raycast_ray;
            bool is_hit = false;
            RaycastHit hit_context;
            common_script.raycast_from_crosshair(this_camera, out raycast_ray, out is_hit, out hit_context);
            if (is_hit)
            {
                // Figure out if raycast hit is sun or planet.
                string hit_gameobject_name = hit_context.collider.gameObject.name.ToLower();
                if (hit_gameobject_name.Contains("sun"))
                {
                    Debug.Log("Creating a planet!");
                    // Pick random planet.
                    int rand_planet_idx = Random.Range(0, n_spawnable_planets);
                    GameObject spawnable_planet = spawnable_planet_templates[rand_planet_idx];
                    // Create planet instance.
                    // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                    GameObject spawnable_planet_inst = Instantiate(spawnable_planet, this_camera.transform.position, Quaternion.identity);
                    spawnable_planet_inst.transform.SetParent(hit_context.collider.gameObject.transform);
                    float starting_scale = 0.001f; //Random.Range(0.1f, 0.5f);
                    spawnable_planet_inst.transform.localScale = new Vector3(starting_scale, starting_scale, starting_scale);
                    // Initialize planet instance.
                    // https://gamedevbeginner.com/how-to-get-a-variable-from-another-script-in-unity-the-right-way/
                    spawnable_planet_inst.GetComponent<SpawnablePlanet>().spawn_ray = raycast_ray;
                    spawnable_planet_inst.GetComponent<SpawnablePlanet>().movement_speed = 30.0f;
                    spawnable_planet_inst.GetComponent<SpawnablePlanet>().max_dist = 1000000.0f;
                    spawnable_planet_inst.GetComponent<SpawnablePlanet>().max_scale = Random.Range(5.0f, 15.0f);
                }
                else if (hit_gameobject_name.Contains("planet"))
                {
                    Debug.Log("Creating satelite!");
                    // Pick random satelite.
                    int rand_satelite_idx = Random.Range(0, n_spawnable_satelites);
                    GameObject spawnable_satelite = spawnable_satelite_templates[rand_satelite_idx];
                    // Create satelite instance.
                    // https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
                    GameObject spawnable_satelite_inst = Instantiate(spawnable_satelite, this_camera.transform.position, Quaternion.identity);
                    // Set satelite as child to planet.
                    spawnable_satelite_inst.transform.SetParent(hit_context.collider.gameObject.transform);
                    // Initialize satelite instance.
                    // https://gamedevbeginner.com/how-to-get-a-variable-from-another-script-in-unity-the-right-way/
                    spawnable_satelite_inst.GetComponent<SpawnableSatelite>().spawn_ray = raycast_ray;
                    spawnable_satelite_inst.GetComponent<SpawnableSatelite>().movement_speed = 30.0f;
                    spawnable_satelite_inst.GetComponent<SpawnableSatelite>().max_dist = 1000000.0f;
                }
            }
            else
            {
                // create sun?
            }
        }

        if (Input.GetKeyDown("f"))
        {
            Debug.Log("Creating SUN!");
            // Create a ray on which sun will be created.
            Vector3 crosshair_pos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray raycast_ray = GetComponent<Camera>().ScreenPointToRay(crosshair_pos);
            Vector3 sun_position = raycast_ray.origin + raycast_ray.direction * 200.0f;
            // Instance random sun.
            int rand_sun_idx = Random.Range(0, n_spawnable_suns);
            GameObject spawnable_sun = spawnable_sun_templates[rand_sun_idx];
            GameObject spawnable_sun_inst = Instantiate(spawnable_sun, sun_position, Quaternion.identity);
            // Rotate.
            float rx = Random.Range(0.0f, 30.0f);
            float rz = Random.Range(0.0f, 30.0f);
            spawnable_sun_inst.transform.Rotate(rx, 0.0f, rz, Space.World); // Sun has no parent.
        }   
    }

    void sphere_movement()
    {
        // Rotation.
        rot_x += cam_sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        rot_y += cam_sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        rot_y = Mathf.Clamp(rot_y, -90.0f, 90.0f);
        transform.localRotation = Quaternion.AngleAxis(rot_x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot_y, Vector3.left);
        // Translation.
        
        //float longitude = Input.GetAxis("Horizontal") * 0.01f;
        //float latitude = Input.GetAxis("Vertical") * 0.01f;
        //transform.localPosition = common_script.move_point_on_sphere(transform.localPosition, longitude, latitude); 
    }

    void free_flying_movement()
    {
        // https://www.youtube.com/watch?v=1bFISMM2g2c
        // https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/
        // Perform rotation based on mouse movement.
        // Note that rotation rotates transform axis!
        rot_x += cam_sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        rot_y += cam_sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        rot_y = Mathf.Clamp(rot_y, -90.0f, 90.0f);
        transform.localRotation = Quaternion.AngleAxis(rot_x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot_y, Vector3.left);
        // Perform translation based on keyboard input.
        if (Input.GetKey(KeyCode.LeftShift)) // https://docs.unity3d.com/ScriptReference/KeyCode.html
        {
            transform.position += cam_speed_fast_fact * cam_speed * transform.forward * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cam_speed_fast_fact * cam_speed * transform.right * Input.GetAxis("Horizontal") * Time.deltaTime;    
        }
        else
        {
            transform.position += cam_speed * transform.forward * Input.GetAxis("Vertical") * Time.deltaTime;
            transform.position += cam_speed * transform.right * Input.GetAxis("Horizontal") * Time.deltaTime;    
        }
    }
}
