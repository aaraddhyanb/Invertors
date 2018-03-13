using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepthMap : MonoBehaviour
{
    public string shaderName;

    void Start()
    { 
    }

    void OnPreRender()
    {
        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.backgroundColor = Color.white;
        cam.renderingPath = RenderingPath.Forward;
        cam.SetReplacementShader(Shader.Find(shaderName), "RenderType");
    }

    void OnPostRender()
    {
    }
}