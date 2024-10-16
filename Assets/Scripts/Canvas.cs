using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.z += 180;
    }

    void Update()
    {
    }
}
