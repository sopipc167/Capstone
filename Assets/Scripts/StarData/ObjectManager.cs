using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class tmpStar : MonoBehaviour
{
    public GameObject GO;
    public float altitude;
    public float azimuth;
    public float distance;

    public tmpStar(GameObject g, float al, float az, float dis)
    {
        GO = g;
        altitude = al;
        azimuth = az;
        distance = dis;
    }
}

public class ObjectManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI headAcc;

    public Button Reset;
    public GameObject starPrefab;
    public GameObject moonPrefab;
    public GameObject sunPrefab;
    public GameObject otherPrefab;
    public GameObject arrow;
    public Transform arCamera;
    float radius = 1000.0f;

    private DataManager dataManager;
    private CompassManager compassManager;

    private List<Constellation> constellations = new List<Constellation>();
    private Dictionary<string, List<GameObject>> constellationObjects = new Dictionary<string, List<GameObject>>();
    private List<GameObject> solarObjects = new List<GameObject>();
    private GameObject sun, mercury, venus, mars, jupiter, saturn, uranus, neptune, pluto, moon;
    private List<GameObject> tmp = new List<GameObject>();
    private List<GameObject> ursaMinor = new List<GameObject>();

    private XROrigin xrOrigin;
    private float trueNorth = 0.0f;
    private Vector3 northDirection = Vector3.zero;
    private bool isInitialized = false;
    private bool isConstellationInitialized = false;
    private Quaternion initialRotation;

    public List<tmpStar> starList = new List<tmpStar>();
    private DistanceManager DM;

    // Start is called before the first frame update
    void Start()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        dataManager = FindAnyObjectByType<DataManager>();
        compassManager = FindAnyObjectByType<CompassManager>();
        DM = FindAnyObjectByType<DistanceManager>();
        if(DM == null)
        {
            Debug.Log("DistanceManager Not available");
        }

        InitializeObjects();
        Debug.Log("Starlist Size : " + starList.Count);
    }

    public void UpdateObjects()
    {
        if (dataManager.celestialObjects.Count >= 98)
        {
            if (!isConstellationInitialized)
            {
                InitializeConstellations();
                isConstellationInitialized = true;
            }

        }
    }

    void Update()
    {
        if (!isInitialized)
        {
            trueNorth = compassManager.GetTrueNorth();
            if (trueNorth == -1.0f) return;

            isInitialized = true;
        }

        if (isInitialized)
        {
            UpdateSolarObjects();
        }
    }



    void UpdateSolarObjects()
    {
        CelestialObject obj;

        obj = dataManager.celestialObjects["sun"];
        UpdateStar(sun, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["mercury"];
        UpdateStar(mercury, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["venus"];
        UpdateStar(venus, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["mars"];
        UpdateStar(mars, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["jupiter"];
        UpdateStar(jupiter, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["saturn"];
        UpdateStar(saturn, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["uranus"];
        UpdateStar(uranus, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["neptune"];
        UpdateStar(neptune, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["pluto"];
        UpdateStar(pluto, obj.alt, obj.az, radius);

        obj = dataManager.celestialObjects["moon"];
        UpdateStar(moon, obj.alt, obj.az, radius);

        UpdateConstellations();
    }

    private void UpdateConstellations()
    {
        Constellation obj;

        obj = dataManager.celestialObjects["orion"].ConvertTo<Constellation>();

        for (int i = 0; i < obj.stars.Count; i++)
        {
            Star star = obj.stars[i];
            UpdateStar(tmp[i], star.alt, star.az, radius);
        }
        UpdateConstellationLine(tmp, obj.lines);

        obj = dataManager.celestialObjects["ursaminor"].ConvertTo<Constellation>();

        for (int i = 0; i < obj.stars.Count; i++)
        {
            Star star = obj.stars[i];
            UpdateStar(ursaMinor[i], star.alt, star.az, radius);
        }
        UpdateConstellationLine(ursaMinor, obj.lines);
    }

    public void UpdateStar(GameObject star, float altitude, float azimuth, float distance)
    {
        float adjustedAzimuth = azimuth - trueNorth;
        Vector3 starPosition = SphericalToCartesian(altitude, adjustedAzimuth, distance);
        starList.Add(new tmpStar(star, altitude, azimuth, distance));
        //Vector3 starPosition = GetStarPosition(altitude, azimuth, distance, northDirection);

        star.transform.position = starPosition;
    }

    private void InitializeObjects()
    {
        sun = Instantiate(sunPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mercury = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mercury.GetComponent<TextScript>().SetText("Mercury");
        venus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        venus.GetComponent<TextScript>().SetText("Venus");
        mars = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mars.GetComponent<TextScript>().SetText("Mars");
        jupiter = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        jupiter.GetComponent<TextScript>().SetText("Jupiter");
        saturn = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        saturn.GetComponent<TextScript>().SetText("Saturn");
        uranus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        uranus.GetComponent<TextScript>().SetText("Uranus");
        neptune = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        neptune.GetComponent<TextScript>().SetText("Neptune");
        pluto = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pluto.GetComponent<TextScript>().SetText("Pluto");
        moon = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        moon.GetComponent<TextScript>().SetText("Moon");
    }

    private void InitializeConstellations()
    {
        Constellation obj;

        obj = dataManager.celestialObjects["orion"].ConvertTo<Constellation>();
        foreach (Star star in obj.stars)
        {
            GameObject newStar = Instantiate(starPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            tmp.Add(newStar);
        }

        InitializeConstellationLine(tmp, obj.lines);

        obj = dataManager.celestialObjects["ursaminor"].ConvertTo<Constellation>();
        foreach (Star star in obj.stars)
        {
            GameObject newStar = Instantiate(starPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ursaMinor.Add(newStar);
        }

        InitializeConstellationLine(ursaMinor, obj.lines);
    }

    private void UpdateConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {


        foreach (List<int> newLine in lines)
        {
            int startIndex = newLine[0];
            LineRenderer line = starObjects[startIndex].GetComponent<LineRenderer>();
            if (line == null) continue;
            line.positionCount = newLine.Count;
            int i = 0;
            newLine.RemoveAt(0);

            foreach (int endIndex in newLine)
            {
                line.SetPosition(i, starObjects[startIndex].transform.position);
                line.SetPosition(i + 1, starObjects[endIndex].transform.position);
                //Debug.Log(string.Format("[{0}]: {1}", endIndex, starObjects[endIndex].transform.position));

                i++;
                startIndex = endIndex;
            }
        }
    }

    void InitializeConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {
        foreach (List<int> newLine in lines)
        {
            int startIndex = newLine[0];
            LineRenderer line = starObjects[startIndex].AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            line.startWidth = 0.5f;
            line.endWidth = 0.5f;
        }

    }

    Vector3 GetStarPosition(float altitude, float azimuth, float distance, Vector3 northDirection)
    {
        float azimuthRad = azimuth * Mathf.Deg2Rad;
        float altitudeRad = altitude * Mathf.Deg2Rad;

        float horizontalProjection = Mathf.Cos(altitudeRad);

        Vector3 starDirection = new Vector3(
            horizontalProjection * Mathf.Sin(azimuthRad),  // X component
            Mathf.Sin(altitudeRad),                        // Y component (up)
            horizontalProjection * Mathf.Cos(azimuthRad)   // Z component
        );

        Quaternion rotationToNorth = Quaternion.FromToRotation(Vector3.forward, northDirection);

        Vector3 rotatedDirection = rotationToNorth * starDirection;
        Vector3 observerPosition = Camera.main.transform.position;
        Vector3 starPosition = observerPosition + (rotatedDirection * distance);

        return starPosition;
    }
    Vector3 SphericalToCartesian(float altitude, float azimuth, float radius)
    {
        float alt_radian = altitude * Mathf.Deg2Rad;
        float az_radian = azimuth * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(alt_radian) * Mathf.Sin(az_radian);
        float y = radius * Mathf.Sin(alt_radian);
        float z = radius * Mathf.Cos(alt_radian) * Mathf.Cos(az_radian);

        return new Vector3(x, y, z);
    }
}