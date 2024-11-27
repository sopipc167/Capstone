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

    void Start()
    {
        layerMask = LayerMask.GetMask("StarLayer");
    }

    public void StartRaycast(DistanceManager dm)
    {
        distanceManager = dm;
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
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭 또는 터치로 선택
            {
                distanceManager.AddSelected(hitObject);
            }
        }
    }
}
