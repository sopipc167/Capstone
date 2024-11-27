using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDistance : MonoBehaviour
{
    Star s1, s2;
    float distance = -1;
    public DistanceManager DM;
    // Start is called before the first frame update
    void Start()
    {
        distance = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float Calculate()
    {
        float radAL1 = DegToRad(s1.alt);
        float radAL2 = DegToRad(s2.alt);
        float radAZ1 = DegToRad(s1.az);
        float radAZ2 = DegToRad(s2.az);
        float dis1 = 1000;
        float dis2 = 1287;
        float deltaAL = radAL1 - radAL2;
        float deltaAZ = radAZ1 - radAZ2;
        float term1 = (float)Math.Pow(dis1, 2) + (float)Math.Pow(dis2, 2);
        float term2 = 2 * dis1 * dis2 * ((float)Math.Sin(radAL1) * (float)Math.Sin(radAL2) + (float)Math.Cos(radAL1) * (float)Math.Cos(radAL2) * (float)Math.Cos(radAZ1 - radAZ2));
        distance = (float)Math.Sqrt(term1 - term2);
        return distance;
    }
    public void SetStars(GameObject star1, GameObject star2)
    {
        Debug.Log($"Stars set for calculation: {star1.name}, {star2.name}");
    }
    float DegToRad(float degrees)
    {
        return degrees * (float)Math.PI / 180.0f;
    }
}
