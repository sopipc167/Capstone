using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour
{
    public Button DistanceCalculate;
    private StarCollider starCollider;
    private DetectorManager detectorManager;
    private CalculateDistance calculator;

    private List<GameObject> selected = new List<GameObject>(); // 선택된 천체 리스트

    void Start()
    {
        starCollider = FindAnyObjectByType<StarCollider>();
        detectorManager = FindAnyObjectByType<DetectorManager>();
        calculator = FindAnyObjectByType<CalculateDistance>();

        DistanceCalculate.onClick.AddListener(StartSelectionProcess);
    }

    private void StartSelectionProcess()
    {
        selected.Clear();
        starCollider.ActivateColliders();
        detectorManager.StartRaycast(this);
    }

    public void AddSelected(GameObject star)
    {
        if (selected.Count < 2 && !selected.Contains(star))
        {
            selected.Add(star);
            Debug.Log($"Selected {star.name}");

            if (selected.Count == 2)
            {
                detectorManager.StopRaycast();
                calculator.SetStars(selected[0], selected[1]);
                float distance = calculator.Calculate();
                Debug.Log($"Distance between stars: {distance}");
                EndSelectionProcess();
            }
        }
    }

    private void EndSelectionProcess()
    {
        starCollider.DeactivateColliders();
        selected.Clear();
    }
}
