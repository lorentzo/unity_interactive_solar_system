using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : MonoBehaviour
{
    [SerializeField] private Material postprocess_material;

    private Camera cam;

    public void Start ()
    {
        cam = GetComponent<Camera>();
        // Add both depth and normals.
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Matrix4x4 view_to_world_matrix = cam.cameraToWorldMatrix;
        postprocess_material.SetMatrix("_viewToWorld", view_to_world_matrix);
        Graphics.Blit(src, dest, postprocess_material);
    }
}
