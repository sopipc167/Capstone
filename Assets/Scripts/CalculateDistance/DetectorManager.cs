using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectorManager : MonoBehaviour
{
    private bool isRaycastActive = false;
    private int layerMask;
    private DistanceManager distanceManager;
    private ObjectManager obj;
    private int hit;
    public Button Confirm;
    private GameObject currentHitObject;

    void Start()
    {
        layerMask = LayerMask.GetMask("Default");
        distanceManager = FindAnyObjectByType<DistanceManager>();
        obj = FindAnyObjectByType<ObjectManager>();
        Confirm.onClick.AddListener(OnConfirmClicked);
        Confirm.gameObject.SetActive(false);
    }

    public void StartRaycast()
    {
        hit = 0;
        isRaycastActive = true;
    }

    public void StopRaycast()
    {
        isRaycastActive = false;
        Confirm.gameObject.SetActive(false);
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
            if (currentHitObject != hitObject)
            {
                currentHitObject = hitObject;
                Confirm.gameObject.SetActive(true);
            }
        }
        else
        {
            currentHitObject = null;
            Confirm.gameObject.SetActive(false);
        }
    }

    private tmpStar GetStarData(GameObject starObject)
    {
        tmpStar star = obj.starList.FirstOrDefault(s => s.gameObject == starObject);
        if (star != null)
        {
            return star;
        }
        else return null;
    }
    private void OnConfirmClicked()
    {
        if (currentHitObject != null)
        {
            tmpStar starData = GetStarData(currentHitObject);
            if (starData != null)
            {
                distanceManager.AddSelected(currentHitObject, starData);
                Confirm.gameObject.SetActive(false);
                currentHitObject = null;
                hit++;
            }
        }
        if (hit == 2) isRaycastActive = false;
    }
}
