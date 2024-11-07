using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    public Material maskMaterial;
    public float sunRadius = 0.5f;
    public float moonRadius = 1.0f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        maskMaterial.SetVector("_SunPosition", sun.transform.position);
        maskMaterial.SetVector("_MoonPosition", moon.transform.position);
        maskMaterial.SetFloat("_SunRadius", sunRadius);
        maskMaterial.SetFloat("_MoonRadius", moonRadius);
    }
}
