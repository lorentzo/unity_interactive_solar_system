using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    // Draw circle around parent.
    // https://docs.unity3d.com/ScriptReference/LineRenderer.html
    // https://answers.unity.com/questions/8338/how-to-draw-a-line-using-script.html
    public void draw_circle(GameObject parent, float r, int n_points, Material mat)
    {
        GameObject line = new GameObject();
        line.transform.SetParent(parent.transform);
        line.AddComponent<LineRenderer>();
        LineRenderer line_renderer = line.GetComponent<LineRenderer>();
        Vector3 center = parent.transform.position;
        float t = 0.0f;
        float dt = (2.0f * Mathf.PI) / n_points;
        line_renderer.positionCount = n_points;
        for (int i = 0; i < n_points; ++i)
        {
            float x = r * Mathf.Cos(t);
            float z = r * Mathf.Sin(t);
            Vector3 tmp_pos = new Vector3(x, 0.0f, z);
            line_renderer.SetPosition(i, tmp_pos);
            t += dt;
        }
        line_renderer.loop = true;
        line_renderer.material = mat;
    }
}
