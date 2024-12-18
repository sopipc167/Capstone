using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using System;
using Unity.XR.CoreUtils;

public class CompassManager : MonoBehaviour
{
    public bool gps_ok = false;

    public GameObject compass;

    float timeDelay = 0.25f;

    public XROrigin xrOrigin;
    private LineRenderer northLine;

    private int index = 0;
    private float trueNorth = 0f;
    private float initialHeading = 0f;
    private int samples = 20;
    private List<float> angles = new List<float>();
    private bool isInitialized = false;


    IEnumerator Start()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
        }
        // Starts the location service.
        Input.location.Start();
        Input.compass.enabled = true;
        Input.gyro.enabled = true;

        northLine = gameObject.AddComponent<LineRenderer>();
        northLine.material = new Material(Shader.Find("Sprites/Default"));
        northLine.startColor = Color.magenta;
        northLine.endColor = Color.magenta;
        northLine.startWidth = 0.05f;
        northLine.endWidth = 0.05f;

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");

            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            // Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            gps_ok = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gps_ok)
        {
            timeDelay -= Time.deltaTime;
            if (!isInitialized)
            {
                if (index < samples)
                {
                    float heading = Input.compass.trueHeading;
                    angles.Add(heading);
                    index += 1;
                }
                else
                {
                    initialHeading = GetAngleMean(angles);
                    if (initialHeading < 0) initialHeading += 360f;

                    DrawTrueNorthLine(initialHeading);
                    isInitialized = true;
                }
            }
            if (timeDelay < 0)
            {
                timeDelay = 0.25f;
                trueNorth = Input.compass.trueHeading;
                compass.transform.localEulerAngles = new Vector3(0, 0, trueNorth);
            }
        }
    }

    private void DrawTrueNorthLine(float angle)
    {
        northLine.positionCount = 2;
        northLine.SetPosition(0, Camera.main.transform.position);
        northLine.SetPosition(1, Quaternion.Euler(0, -angle, 0) * Vector3.forward * 100);
    }

    public float GetTrueNorth()
    {
        if (isInitialized) return initialHeading;
        return -1.0f;
    }

    private float GetAngleMean(List<float> angles)
    {
        float sinSum = 0f;
        float cosSum = 0f;

        foreach (float angle in angles)
        {
            float radians = angle * Mathf.Deg2Rad;
            sinSum += Mathf.Sin(radians);
            cosSum += Mathf.Cos(radians);
        }
        sinSum /= angles.Count;
        cosSum /= angles.Count;

        float meanRadians = Mathf.Atan2(sinSum, cosSum);
        return meanRadians * Mathf.Rad2Deg;
    }
}