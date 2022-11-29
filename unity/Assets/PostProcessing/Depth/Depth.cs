using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour
{
    [SerializeField] private Material postprocess_material;

    public void Start ()
    {
        Camera cam = GetComponent<Camera>();
        // Add another mode.
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postprocess_material);
    }
}
