using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBehavior : MonoBehaviour
{
    private float selfRotateAngle = 0f;
    public static float selfRotateSpeed = 10f; //���� �ӵ�
    float rotationAxis = 30.15f; //������ ����

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