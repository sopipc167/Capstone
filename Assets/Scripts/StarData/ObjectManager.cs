using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;

public class ObjectManager : MonoBehaviour
{
    public TextMeshProUGUI text;

    public GameObject starPrefab;
    public GameObject moonPrefab;
    public Transform arCamera;
    float radius = 1000.0f;

    private DataManager dataManager;

    private float sun_alt = 0.0f, sun_az = 0.0f;
    private float moon_alt = 0.0f, moon_az = 0.0f;
    private float trueHeading = 0.0f;
    private bool compassInitialized = false;
    private GameObject moon, sun;
    private List<Vector3> points = new List<Vector3>();
    private List<GameObject> stars = new List<GameObject>();

    private XROrigin xrOrigin;

    // Start is called before the first frame update
    void Start()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        dataManager = FindAnyObjectByType<DataManager>();
        Input.location.Start();
        Input.compass.enabled = true;

        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
#endif
        trueHeading = Input.compass.trueHeading;
        sun = Instantiate(moonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        moon = Instantiate(moonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void UpdateObjects()
    {
        if (dataManager.celestialObjects.Count >= 98)
        {
            CelestialObject obj = dataManager.celestialObjects["sun"];
            sun_alt = obj.alt;
            sun_az = obj.az;

            obj = dataManager.celestialObjects["moon"];
            moon_alt = obj.alt;
            moon_az = obj.az;
        }
    }

    void Update()
    {
        if (!compassInitialized && Input.compass.headingAccuracy > 0)
        {
            trueHeading = Input.compass.trueHeading;
            compassInitialized = true;
        }

        if (compassInitialized)
        {
            xrOrigin.transform.rotation = Quaternion.Euler(0, trueHeading, 0);
        }

        text.text = string.Format("TrueHeading: {0}", trueHeading);
        UpdateStar(sun, sun_alt, sun_az, radius);
        UpdateStar(moon, moon_alt, moon_az, radius);
    }

    void Sagittarius()
    {
        PlaceStar(-19.4f, 233.2f, radius);  // alf
        PlaceStar(-21.6f, 229.9f, radius);  // bet
        PlaceStar(-15.3f, 228.6f, radius);  // iot
        PlaceStar(-10.5f, 233.4f, radius);  // tet
        PlaceStar(-5.6f, 241.0f, radius);  // ome
        PlaceStar(-15.1f, 246.5f, radius);  // tau
        PlaceStar(-17.1f, 245.2f, radius);  // zet
        PlaceStar(-16.5f, 249.2f, radius);  // sig
        PlaceStar(-14.7f, 252.4f, radius);  // nu
        PlaceStar(-12.1f, 251.8f, radius);  // omi
        PlaceStar(-10.7f, 251.7f, radius);  // pi
        PlaceStar(-6.6f, 252.6f, radius);  // rho
        PlaceStar(-18.6f, 249.9f, radius);  // phi
        PlaceStar(-21.1f, 253.5f, radius);  // lam
        PlaceStar(-26.4f, 245.6f, radius);  // eps
        PlaceStar(-24.7f, 250.3f, radius);  // del
        PlaceStar(-28.7f, 243.9f, radius);  // eta
        PlaceStar(-21.5f, 259.4f, radius);  // mu
        PlaceStar(-27.9f, 251.6f, radius);  // gam

        DrawConstellationLine(new List<int> { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11 });
        DrawConstellationLine(new List<int> { 0, 2 });
        DrawConstellationLine(new List<int> { 7, 12, 13, 17 });
        DrawConstellationLine(new List<int> { 5, 6, 12, 15, 18, 14, 16 });
        DrawConstellationLine(new List<int> { 6, 14, 15, 13 });
    }

    void DrawConstellationLine(List<int> lines)
    {
        int startIndex = lines[0];
        LineRenderer line = stars[startIndex].AddComponent<LineRenderer>();
        line.startColor = Color.yellow;
        line.endColor = Color.yellow;
        line.positionCount = lines.Count;
        line.startWidth = 2f;
        line.endWidth = 2f;
        int i = 0;
        lines.RemoveAt(0);

        foreach (int endIndex in lines)
        {
            line.SetPosition(i, points[startIndex]);
            line.SetPosition(i+1, points[endIndex]);

            i++;
            startIndex = endIndex;
        }
    }

    public void UpdateStar(GameObject star, float altitude, float azimuth, float distance)
    {
        //float adjustedAzimuth = azimuth - trueHeading;

        Vector3 starPosition = SphericalToCartesian(altitude, azimuth, distance);

        star.transform.position = starPosition;
    }

    public void PlaceStar(float altitude, float azimuth, float distance)
    {
        Vector3 starPosition = WorldPosition(SphericalToCartesian(altitude, azimuth, distance));
        points.Add(starPosition);

        GameObject star = Instantiate(starPrefab, starPosition, Quaternion.identity);
        stars.Add(star);
    }

    Vector3 WorldPosition(Vector3 position)
    {
        Vector3 worldPosition = arCamera.position + arCamera.rotation * position;

        return worldPosition;
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
