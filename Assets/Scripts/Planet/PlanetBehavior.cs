using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBehavior : MonoBehaviour
{
    private float selfRotateAngle = 0f;
    public static float selfRotateSpeed = 10f; //자전 속도
    float rotationAxis = 30.15f; //자전축 각도

    void Start()
    {
        
        transform.rotation = Quaternion.Euler(0, 0, rotationAxis);
    }

    // Update is called once per frame
    void Update()
    {
        // get desired position

        selfRotateAngle = selfRotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, selfRotateAngle);
    }
}