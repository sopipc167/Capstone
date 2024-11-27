using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectorManager : MonoBehaviour
{
    private bool isRaycastActive = false;
    private int layerMask;
    private DistanceManager distanceManager;
    public Button Confirm;

    void Start()
    {
        layerMask = LayerMask.GetMask("Default");
        distanceManager = FindAnyObjectByType<DistanceManager>();
    }

    public void StartRaycast()
    {
        isRaycastActive = true;
    }

    public void StopRaycast()
    {
        isRaycastActive = false;
    }

    void Update()
    {
        if (isRaycastActive)
        {
            PerformRaycast();
        }
    }

    private void PerformRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (Input.GetMouseButtonDown(0)) // 클릭 시 선택
            {
                Star starData = GetStarData(hitObject);
                if (starData != null)
                {
                    distanceManager.AddSelected(hitObject, starData);
                }
            }
        }
    }

    private Star GetStarData(GameObject starObject)
    {
        ObjectManager objectManager = FindAnyObjectByType<ObjectManager>();
        if (objectManager != null && objectManager.starList.ContainsKey(starObject))
        {
            return objectManager.starList[starObject];
        }
        return null;
    }
}
