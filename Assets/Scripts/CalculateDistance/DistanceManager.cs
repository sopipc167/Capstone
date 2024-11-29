using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour
{
    public GameObject DetectionPanel;
    private StarCollider starCollider;
    private DetectorManager detectorManager;

    private Dictionary<GameObject, tmpStar> selected = new Dictionary<GameObject, tmpStar>(); // 선택된 천체 리스트

    void Start()
    {
        Debug.Log("Distance Manager Acitve");
        starCollider = FindAnyObjectByType<StarCollider>(); 
        detectorManager = FindAnyObjectByType<DetectorManager>();
    }

    public void StartSelectionProcess()
    {
        Debug.Log("Distance Button Pressed");
        selected.Clear();
        starCollider.ActivateColliders();
        detectorManager.StartRaycast();
        DetectionPanel.gameObject.SetActive(true);
    }

    public void AddSelected(GameObject starObject, tmpStar starData)
    {
        if (selected.Count < 2 && !selected.ContainsKey(starObject))
        {
            selected[starObject] = starData;
            Debug.Log($"Selected: {starObject.name}");

            if (selected.Count == 2)
            {
                DetectionPanel.gameObject.SetActive(false);
                CalculateDistanceBetweenSelected();
                EndSelectionProcess();
            }
        }
    }

    private void CalculateDistanceBetweenSelected()
    {
        var selectedStars = new List<tmpStar>(selected.Values);
        if (selectedStars.Count == 2)
        {
            tmpStar s1 = selectedStars[0];
            tmpStar s2 = selectedStars[1];

            float distance = CalculateDistance(s1, s2);
            Debug.Log($"Distance between selected stars: {distance}");
        }
    }

    private float CalculateDistance(tmpStar s1, tmpStar s2)
    {
        float radAL1 = Mathf.Deg2Rad * s1.star.alt;
        float radAL2 = Mathf.Deg2Rad * s2.star.alt;
        float radAZ1 = Mathf.Deg2Rad * s1.star.az;
        float radAZ2 = Mathf.Deg2Rad * s2.star.az;

        float dis1 = s1.star.distance;
        float dis2 = s2.star.distance;

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
