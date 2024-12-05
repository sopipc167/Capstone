using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using TouchPhase = UnityEngine.TouchPhase;

public class InputManager : MonoBehaviour
{
    public Camera arCamera;

    private float minDistance = 200.0f;
    private float currentZoom = 0.0f;
    private float zoomSpeed = 0.4f;
    private float prevDistance;
    private float currentDistance;

    private ObjectManager objectManager;
    public TextMeshProUGUI objectText;

    private void Start()
    {
        arCamera = Camera.main;
        objectManager = FindAnyObjectByType<ObjectManager>();
    }

    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (currentZoom >= 4)
        {
            info.SetActive(true);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if (go.transform.parent != null)
                {
                    objectText.text = go.transform.parent.name + " " + go.name;
                }
                else
                {
                    objectText.text = go.name;
                }
            }
        }
        else if (currentZoom < 4)
            info.SetActive(false);

        if (Input.touchCount >= 2)
        {
            firstTouch = Input.GetTouch(0);
            secondTouch = Input.GetTouch(1);

            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
            {
                prevDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
            }

            if (firstTouch.phase == TouchPhase.Moved &&
                secondTouch.phase == TouchPhase.Moved)
            {
                currentDistance = Vector2.Distance(firstTouch.position, secondTouch.position);

                float deltaDistance = currentDistance - prevDistance;

                UpdateZoom(deltaDistance);
                objectManager.UpdateObjects();

                prevDistance = currentDistance;
            }
        }
    }
    private void UpdateZoom(float distance)
    {
        Vector3 direction = -new Vector3(0, 0, 1);
        Vector3 targetPosition = objectManager.originObject.transform.localPosition + direction * distance * zoomSpeed;

        if (targetPosition.z > 0)
        {
            targetPosition = Vector3.zero;
        }
        else if (targetPosition.z < minDistance - 1000.0f)
        {
            targetPosition = new Vector3(0, 0, minDistance - 1000.0f);
        }

        objectManager.originObject.transform.localPosition = targetPosition;
        currentZoom = (1000.0f) / (1000.0f + targetPosition.z);
    }
}