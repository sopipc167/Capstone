using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    //public Material maskMaterial;
    public float sunRadius = 0.5f;
    public float moonRadius = 1.0f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_SunPosition", sun.transform.position);
        Shader.SetGlobalVector("_MoonPosition", moon.transform.position); 
        Shader.SetGlobalFloat("_SunRadius", sunRadius);
        Shader.SetGlobalFloat("_MoonRadius", moonRadius);


    }
}
