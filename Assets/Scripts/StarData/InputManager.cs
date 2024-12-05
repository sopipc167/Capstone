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

    private Touch firstTouch;
    private Touch secondTouch;

    private float minDistance = 100.0f;
    private float currentZoom = 0.0f;
    private float zoomSpeed = 0.4f;
    private float prevDistance;
    private float currentDistance;
    [SerializeField]
    GameObject info;

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
                string name = hit.collider.gameObject.name;
                objectText.text = name;
                StartCoroutine(Search(name));
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
    private IEnumerator Search(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/api/stellar/" + url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.responseCode == 200)
            {
                info.GetComponent<Info_PanelControler>().getData(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError(webRequest.error);
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