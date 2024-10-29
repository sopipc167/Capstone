using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TouchManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private GameObject moonPrefab;
    [SerializeField]
    private GameObject sunPrefab;
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    [SerializeField]
    private Button sunSpawnButton;


    private GameObject sunInstance;
    private GameObject moonInstance;
    
    private bool touched = false;
    private GameObject selectedObj;
    private ColorGrading colorGrading;

    private float maxDistance = 0.06f;
    private Material sunMaterial;
    private Color originalEmissionColor;

    private float sunDiameter = 0.06f;

    private float moonFixedOffset = 0.01f;


    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);

        // Transform sphereTransform = sunPrefab.transform.Find("Shpere");
        // Renderer sphereRenderer = sphereTransform.GetComponent<Renderer>();
        // sunMaterial = sphereRenderer.material;
        // if(sunMaterial.HasProperty("_EmissionColor"))
        // {
        //     originalEmissionColor = sunMaterial.GetColor("_EmissionColor");
        //     sunMaterial.EnableKeyword("_EMISSION");
        // }

        sunSpawnButton.onClick.AddListener(SpawnSun);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if(touch.phase == TouchPhase.Began) 
        {
            if (moonInstance == null)
            {
                // Instantiate moonPrefab at touch position for the first time
                Vector3 spawnPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, sunInstance.transform.position.z + moonFixedOffset));
                moonInstance = Instantiate(moonPrefab, spawnPosition, Quaternion.identity);
                touched = true;
            }
            else
            {
                // Raycast to check if selectedObj was touched
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == moonInstance)
                {
                    touched = true; // Set to true if the existing object is touched
                }
            }
        }
        if(touch.phase == TouchPhase.Ended)
        {
            touched = false;
        }
        if (touched && moonInstance != null)
        {
            Vector3 newPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, sunInstance.transform.position.z + moonFixedOffset));
            moonInstance.transform.position = newPosition;
        }

        AdjustBrightness();
    }

    private void SpawnSun()
    {
        if(sunInstance == null)
        {
            sunInstance = Instantiate(sunPrefab, new Vector3(0,0.2f,0.5f), Quaternion.identity);
            sunInstance.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        }
    }

    private void AdjustBrightness()
    {
        if (moonInstance == null || sunInstance == null || colorGrading == null)
            return;

        // Calculate the direction from sun to moon and distance
        Vector3 sunToMoon = moonInstance.transform.position - sunInstance.transform.position;
        Vector3 sunToCamera = arCamera.transform.position - sunInstance.transform.position;

        // --FIRST--
        // // Check if moon is roughly in front of the sun from camera's perspective
        // float distanceThreshold = 0.2f; // Adjust this value based on desired proximity
        // bool isMoonInFrontOfSun = Vector3.Dot(sunToCamera.normalized, sunToMoon.normalized) > 0.98f && sunToMoon.magnitude < distanceThreshold;

        // if (isMoonInFrontOfSun)
        // {
        //     colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, -3f, 0.5f * Time.deltaTime);
        //     colorGrading.saturation.value = -50f;
        // }
        // else
        // {
        //     colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, 0f, 0.5f * Time.deltaTime);
        //     colorGrading.saturation.value = 0f;
        // }

        // //--SECOND--
        // // Calculate the distance between sun and moon
        // float currentDistance = Vector3.Distance(selectedObj.transform.position, sunPrefab.transform.position);

        // // Calculate the normalized "eclipse factor" based on distance
        // float eclipseFactor = Mathf.Clamp01(1 - (currentDistance / maxDistance));

        // // Adjust post-process settings based on the eclipse factor
        // colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, eclipseFactor);
        // colorGrading.saturation.value = Mathf.Lerp(0f, -25f, eclipseFactor);

        // sunMaterial.SetColor("_EmissionColor", originalEmissionColor);


        // Calculate the projection of sunToMoon onto sunToCamera
        float projectionLength = Vector3.Dot(sunToMoon, sunToCamera.normalized);
        Vector3 projectionPoint = sunInstance.transform.position + sunToCamera.normalized * projectionLength;

        // Calculate distance from moon to the projected point on the line
        float distanceFromLine = Vector3.Distance(moonInstance.transform.position, projectionPoint);

        // If the projection point is in front of the sun and within the sun's diameter range, start eclipse effect
        if (projectionLength > 0 && distanceFromLine <= sunDiameter)
        {
            // Calculate the "eclipse factor" based on how much moon is covering the sun
            float eclipseFactor = Mathf.Clamp01(1 - (distanceFromLine / sunDiameter));

            // Adjust post-process settings based on the eclipse factor
            colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, eclipseFactor);
            colorGrading.saturation.value = Mathf.Lerp(0f, -25f, eclipseFactor);

        }
        else
        {
            // Reset brightness when moon is not in front of the sun-camera line
            colorGrading.postExposure.value = 0f;
            colorGrading.saturation.value = 0f;
        }

    }
}

