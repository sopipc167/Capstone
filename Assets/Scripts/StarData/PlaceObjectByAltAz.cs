using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceObjectByAltAz : MonoBehaviour
{
    public GameObject starPrefab;
    public Transform arCamera;
    float radius = 1000.0f;

    private List<Vector3> points = new List<Vector3>();
    private List<GameObject> stars = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Input.compass.enabled = true;

        Sagittarius();
    }

    void Update()
    {

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

        DrawConstellationLine(1, new int[] { 2, 3, 4, 5, 7, 8, 9, 10, 11 });
        DrawConstellationLine(0, new int[] { 2 });
        DrawConstellationLine(7, new int[] { 12, 13, 17 });
        DrawConstellationLine(5, new int[] { 6, 12, 15, 18, 14, 16 });
        DrawConstellationLine(6, new int[] { 14, 15, 13 });
    }

    void DrawConstellationLine(int startPoint, int[] endPoints)
    {
        LineRenderer line = stars[startPoint].AddComponent<LineRenderer>();
        line.positionCount = endPoints.Length + 1;
        line.startWidth = 2f;
        line.endWidth = 2f;
        int i = 0;
        int startIndex = startPoint;

        foreach (int endIndex in endPoints)
        {
            line.SetPosition(i, points[startIndex]);
            line.SetPosition(i+1, points[endIndex]);

            i++;
            startIndex = endIndex;
        }
    }

    public void PlaceStar(float altitude, float azimuth, float distance)
    {
        float compassHeading = Input.compass.trueHeading;
        float adjustedAzimuth = azimuth - compassHeading;


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
