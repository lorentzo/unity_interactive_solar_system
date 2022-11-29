using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertColor : MonoBehaviour
{
    [SerializeField] private Material postprocess_material;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, postprocess_material);
    }
}
