using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using static UnityEngine.GraphicsBuffer;

public class ObjectManager : MonoBehaviour
{
    public Camera arCamera;
    public GameObject starPrefab;
    public GameObject sunPrefab;
    public GameObject otherPrefab;
    public GameObject originObject;
    public GameObject pointerObject;
    public Toggle lineToggle;
    public Toggle eclipticToggle;
    public Vector3 origin = Vector3.zero;
    public float radius = 1000.0f;
    public float lineWidth = 0.5f;

    private DataManager dataManager;
    private CompassManager compassManager;

    private GameObject sun, mercury, venus, mars, jupiter, saturn, uranus, neptune, pluto, moon;
    private List<List<GameObject>> constellationObjects = new List<List<GameObject>>();
    private Dictionary<string, List<GameObject>> constellations = new Dictionary<string, List<GameObject>>();
    private List<Vector3> ecliptic = new List<Vector3>();
    private GameObject eclipticPoint;
    private GameObject trackingObject;

    private float screenWidth, screenHeight;
    private float edgeOffset = 25.0f;
    private float pointerMargin = 100.0f;

    private float trueNorth = 0.0f;
    private bool isInitialized = false;
    private bool isConstellationInitialized = false;

    void Start()
    {
        dataManager = FindAnyObjectByType<DataManager>();
        compassManager = FindAnyObjectByType<CompassManager>();

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        InitializeObjects();
        
    }

    public void UpdateObjects()
    {
        if (!isConstellationInitialized)
        {
            NewInitializeConstellations();
            isConstellationInitialized = true;
        }
        /*
        if (isInitialized)
        {
            UpdateSolarObjects();
            UpdateConstellations();
            DrawEclipticLine();
        }
        */
    }

    void Update()
    {
        if (!isInitialized)
        {
            trueNorth = compassManager.GetTrueNorth();
            if (trueNorth == -1.0f) return;

            NewInitializeEclipticLine();
            isInitialized = true;
        }

        if (isInitialized)
        {
            UpdateSolarObjects();
            NewUpdateConstellations();

            DrawEclipticLine();
        }
        
        if (trackingObject)
        {
            Vector3 targetScreenPosition = arCamera.WorldToScreenPoint(trackingObject.transform.position);
            Vector3 targetPositionInScreen = new Vector3(targetScreenPosition.x, targetScreenPosition.y, 0);
            Vector3 pointerPositionInScreen = new Vector3(pointerObject.transform.position.x, pointerObject.transform.position.y, 0);
            Vector3 pointerVector = new Vector3(1, 0, 0);
            Vector3 trackingVector;

            if (targetScreenPosition.z > 0)
            {
                trackingVector = targetPositionInScreen - pointerPositionInScreen;

                if (targetScreenPosition.x <= pointerMargin) targetPositionInScreen.x = pointerMargin;
                if (targetScreenPosition.y <= pointerMargin) targetPositionInScreen.y = pointerMargin;
                if (targetScreenPosition.x >= screenWidth - pointerMargin) targetPositionInScreen.x = screenWidth - pointerMargin;
                if (targetScreenPosition.y >= screenHeight - pointerMargin) targetPositionInScreen.y = screenHeight - pointerMargin;
            }
            else
            {
                targetPositionInScreen.y = screenHeight - targetScreenPosition.y;
                targetPositionInScreen.x = -targetScreenPosition.x;
                trackingVector = targetPositionInScreen - pointerPositionInScreen;

                if (targetScreenPosition.x < screenWidth / 2) targetPositionInScreen.x = screenWidth - pointerMargin;
                else targetPositionInScreen.x = pointerMargin;

                if (targetScreenPosition.y >= screenHeight) targetPositionInScreen.y = screenHeight - pointerMargin;
                else if (targetScreenPosition.y <= 0) targetPositionInScreen.y = pointerMargin;
            }

            pointerObject.transform.localEulerAngles = new Vector3(0, 0, Vector3.Angle(pointerVector, trackingVector));
            pointerObject.transform.position = targetPositionInScreen;
            pointerObject.SetActive(true);
        }
    }
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
    }

    // ���ڸ� ��ġ ������Ʈ
    private void NewUpdateConstellations()
    {
        foreach (KeyValuePair<string, CelestialObject> kvp in dataManager.celestialObjects)
        {
            if (kvp.Value.type != "constellation")
            {
                continue;
            }

            Constellation obj = kvp.Value.ConvertTo<Constellation>();
            int i = 0;
            foreach (Star star in obj.stars)
            {
                UpdateStar(constellations[obj.name][i], star.alt, star.az, radius);
                i++;
            }
            UpdateConstellationLine(constellations[obj.name], obj.lines);
        }
    }

    // õü ��ġ ������Ʈ
    public void UpdateStar(GameObject star, float altitude, float azimuth, float distance)
    {
        float adjustedAzimuth = azimuth - trueNorth;
        Vector3 starPosition = SphericalToCartesian(altitude, adjustedAzimuth, distance);

        star.transform.position = starPosition;
        star.transform.LookAt(arCamera.transform.position);
    }

    // �¾�� õü ������Ʈ �ʱ�ȭ
    private void InitializeObjects()
    {
        sun = Instantiate(sunPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        sun.name = "Sun";
        trackingObject = sun;
        mercury = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mercury.GetComponent<TextScript>().SetText("Mercury");
        mercury.name = "Mercury";
        venus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        venus.GetComponent<TextScript>().SetText("Venus");
        venus.name = "Venus";
        mars = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        mars.GetComponent<TextScript>().SetText("Mars");
        mars.name = "Mars";
        jupiter = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        jupiter.GetComponent<TextScript>().SetText("Jupiter");
        jupiter.name = "Jupiter";
        saturn = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        saturn.GetComponent<TextScript>().SetText("Saturn");
        saturn.name = "Saturn";
        uranus = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        uranus.GetComponent<TextScript>().SetText("Uranus");
        uranus.name = "Uranus";
        neptune = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        neptune.GetComponent<TextScript>().SetText("Neptune");
        neptune.name = "Neptune";
        pluto = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pluto.GetComponent<TextScript>().SetText("Pluto");
        pluto.name = "Pluto";
        moon = Instantiate(otherPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        moon.GetComponent<TextScript>().SetText("Moon");
        moon.name = "Moon";
    }

    // ���ڸ� ������Ʈ �ʱ�ȭ
    private void NewInitializeConstellations()
    {
        foreach (KeyValuePair<string, CelestialObject> kvp in dataManager.celestialObjects)
        {
            if (kvp.Value.type != "constellation")
            {
                continue;
            }

            Constellation obj = kvp.Value.ConvertTo<Constellation>();
            GameObject parentObject = new GameObject(obj.name);
            if (!constellations.ContainsKey(obj.name))
            {
                List<GameObject> stars = new List<GameObject>();
                foreach (Star star in obj.stars)
                {
                    GameObject newStar = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
                    newStar.name = star.name;
                    float localscale = UnityEngine.Random.Range(1.0f, 4.0f);
                    newStar.transform.localScale = new Vector3(localscale, localscale, localscale);
                    newStar.transform.parent = parentObject.transform;
                    stars.Add(newStar);
                }
                constellations[obj.name] = stars;
                InitializeConstellationLine(constellations[obj.name], obj.lines);
            }
        }
    }

    // ���ڸ� �� ������Ʈ
    private void UpdateConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {
        foreach (List<int> newLine in lines)
        {
            List<int> tmp = newLine.ToList();

            int startIndex = tmp[0];
            LineRenderer line = starObjects[startIndex].GetComponent<LineRenderer>();
            if (line == null) continue;
            line.positionCount = newLine.Count;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            int i = 0;
            tmp.RemoveAt(0);

            foreach (int endIndex in tmp)
            {
                line.SetPosition(i, starObjects[startIndex].transform.position);
                line.SetPosition(i + 1, starObjects[endIndex].transform.position);

                i++;
                startIndex = endIndex;
            }
        }
    }

    // ���ڸ� �� ǥ���ϱ� ���� LineRenderer �ʱ�ȭ
    void InitializeConstellationLine(List<GameObject> starObjects, List<List<int>> lines)
    {
        foreach (List<int> newLine in lines)
        {
            int startIndex = newLine[0];
            LineRenderer line = starObjects[startIndex].AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
        }
    }

    private void DrawEclipticLine()
    {
        LineRenderer line = eclipticPoint.GetComponent<LineRenderer>();
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            line.SetPosition(i, ecliptic[i]);
        }
        line.SetPosition(line.positionCount - 1, ecliptic[0]);
    }

    private void InitializeEclipticLine()
    {
        int pointsCount = 365;
        float orbitRadius = 1000.0f;
        eclipticPoint = new GameObject();
        LineRenderer line = eclipticPoint.AddComponent<LineRenderer>();
        line.positionCount = pointsCount + 1;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(255, 184, 17);
        line.endColor = new Color(255, 184, 17);
        line.startWidth = 3f;
        line.endWidth = 3f;

        for (int i = 0; i < pointsCount; i++)
        {
            float angle = i * (360f / 365f);
            float x = orbitRadius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float y = Mathf.Sin(Mathf.Deg2Rad * 23.5f) * Mathf.Cos(Mathf.Deg2Rad * angle) * orbitRadius;
            float z = orbitRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 position = new Vector3(x, y, z);

            ecliptic.Add(position);
        }

        Quaternion rotation = Quaternion.Euler(0, trueNorth, 0);
        for (int i = 0; i < pointsCount; i++)
        {
            ecliptic[i] = rotation * ecliptic[i];
            ecliptic[i] = Quaternion.Euler(0, 90, 0) * ecliptic[i];
            ecliptic[i] += Quaternion.Euler(0, -trueNorth, 0) * Vector3.forward * 50;
        }
    }

    private void NewInitializeEclipticLine()
    {
        int pointsCount = dataManager.sunPositions.Count;
        float radius = 1000.0f;
        eclipticPoint = new GameObject();
        LineRenderer line = eclipticPoint.AddComponent<LineRenderer>();
        line.positionCount = pointsCount + 1;
        line.startWidth = 3f;
        line.endWidth = 3f;

        for (int i = 0; i < pointsCount; i++)
        {
            Dictionary<string, float> pos = dataManager.sunPositions[i];
            Vector3 position = SphericalToCartesian(pos["alt"], pos["az"] - trueNorth, radius);
            ecliptic.Add(position);
        }
    }

    public void LineToggle(bool isOn)
    {
        int i = 0;
        foreach (KeyValuePair<string, CelestialObject> kvp in dataManager.celestialObjects)
        {
            if (kvp.Value.type != "constellation")
            {
                continue;
            }

            Constellation obj = kvp.Value.ConvertTo<Constellation>();

            for (int j = 0; j < obj.lines.Count; j++)
            {
                LineRenderer line = constellations[obj.name][obj.lines[j][0]].GetComponent<LineRenderer>();
                if (line == null) continue;

                if (isOn)
                {
                    line.enabled = true;
                }
                else
                {
                    line.enabled = false;
                }
            }
            i++;
        }
    }

    public void EclipticToggle(bool isOn)
    {
        LineRenderer line = eclipticPoint.GetComponent<LineRenderer>();
        if (line == null) return;
        if (isOn)
        {
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }

    // ������ �������� ���� ��ǥ��� ��ȯ
    Vector3 SphericalToCartesian(float altitude, float azimuth, float radius)
    {
        float alt_radian = altitude * Mathf.Deg2Rad;
        float az_radian = azimuth * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(alt_radian) * Mathf.Sin(az_radian);
        float y = radius * Mathf.Sin(alt_radian);
        float z = radius * Mathf.Cos(alt_radian) * Mathf.Cos(az_radian);

        return new Vector3(x, y, z) + originObject.transform.position;
    }
}
