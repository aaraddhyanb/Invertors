using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveShadows : MonoBehaviour {

    public GameObject cameraView;
    public float bias;
    public float blur;
    public int textureSize;

    Material material;
    Camera cam;

    // Use this for initialization
    void Start()
    {
        material = GetComponent<Renderer>().material;
        cam = cameraView.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update ()
    {
        Vector3 position = cam.transform.position;
        Vector4 lightPosition = new Vector4(position.x, position.y, position.z, 1.0f);

        material.SetMatrix("_LightProjectionMat", cam.projectionMatrix * cam.worldToCameraMatrix);
        material.SetVector("_LightPosition", lightPosition);
        material.SetFloat("_NearClip", cam.nearClipPlane);
        material.SetFloat("_FarClip", cam.farClipPlane);
        material.SetFloat("_Bias", bias);
        material.SetFloat("_Blur", blur);
        material.SetInt("_TexSize", textureSize);
    }
}
