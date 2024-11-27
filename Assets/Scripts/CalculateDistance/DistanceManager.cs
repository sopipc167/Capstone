using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour
{
    public Button DistanceCalculate;
    public GameObject DetectionPanel;
    private StarCollider starCollider;
    private DetectorManager detectorManager;

    private Dictionary<GameObject, Star> selected = new Dictionary<GameObject, Star>(); // 선택된 천체 리스트

    void Start()
    {
        starCollider = FindAnyObjectByType<StarCollider>();
        detectorManager = FindAnyObjectByType<DetectorManager>();

        DistanceCalculate.onClick.AddListener(StartSelectionProcess);
    }

    private void StartSelectionProcess()
    {
        selected.Clear();
        starCollider.ActivateColliders();
        detectorManager.StartRaycast();
        DetectionPanel.SetActive(true);
    }

    public void AddSelected(GameObject starObject, Star starData)
    {
        if (selected.Count < 2 && !selected.ContainsKey(starObject))
        {
            selected[starObject] = starData;
            Debug.Log($"Selected: {starObject.name}");

            if (selected.Count == 2)
            {
                detectorManager.StopRaycast();
                DetectionPanel.gameObject.SetActive(false);
                CalculateDistanceBetweenSelected();
                EndSelectionProcess();
            }
        }
    }

    private void CalculateDistanceBetweenSelected()
    {
        var selectedStars = new List<Star>(selected.Values);
        if (selectedStars.Count == 2)
        {
            Star s1 = selectedStars[0];
            Star s2 = selectedStars[1];

            float distance = CalculateDistance(s1, s2);
            Debug.Log($"Distance between selected stars: {distance}");
        }
    }

    private float CalculateDistance(Star s1, Star s2)
    {
        float radAL1 = Mathf.Deg2Rad * s1.alt;
        float radAL2 = Mathf.Deg2Rad * s2.alt;
        float radAZ1 = Mathf.Deg2Rad * s1.az;
        float radAZ2 = Mathf.Deg2Rad * s2.az;

        float dis1 = 1000; // Example distances
        float dis2 = 1287;

        float deltaAL = radAL1 - radAL2;
        float deltaAZ = radAZ1 - radAZ2;

        float term1 = dis1 * dis1 + dis2 * dis2;
        float term2 = 2 * dis1 * dis2 * (Mathf.Sin(radAL1) * Mathf.Sin(radAL2) +
                                         Mathf.Cos(radAL1) * Mathf.Cos(radAL2) * Mathf.Cos(deltaAZ));

        return Mathf.Sqrt(term1 - term2);
    }

    private void EndSelectionProcess()
    {
        starCollider.DeactivateColliders();
        selected.Clear();
    }
}
