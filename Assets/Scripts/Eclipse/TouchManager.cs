using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private GameObject moonPrefab;
    [SerializeField]
    private GameObject sunPrefab;
    private GameObject sunGlow;
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    [SerializeField]
    private Button createMoonButton;
    [SerializeField]
    private Button totalEclipseButton;
    [SerializeField]
    private Button annularEclipseButton;
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private Button EclipseButton;

    private const float MOVE_DISTANCE = 0.02f;
    private const float MAX_X_RANGE = 1.3f;  // 최대 X 좌표
    private const float MIN_X_RANGE = -1.3f; // 최소 X 좌표
    private const float MIN_SCALE = 0.5f;   // Sun Glow 최소 스케일
    private const float MAX_SCALE = 1.0f;    // Sun Glow 최대 스케일

    private GameObject sunInstance;
    private GameObject moonInstance;
    
    private bool touched = false;
    private ColorGrading colorGrading;


    private float sunDiameter = 0.5f;
    private float moonFixedOffset = -0.03f;
    private float moonZPos;
    private Vector3 touchOffset;

    bool isTotalEclipse = true;

    private bool isLeftButtonPressed = false;
    private bool isRightButtonPressed = false;

    void Start()
    {
        postProcessVolume.profile.TryGetSettings(out colorGrading);

        // createMoonButton.onClick.AddListener(() => {
        //     if (moonInstance == null)
        //     {
        //         Vector3 spawnPosition = new Vector3(1.5f, 6f, 14.97f);
        //         moonInstance = Instantiate(moonPrefab, spawnPosition, Quaternion.identity);
        //     }
        // });


        EclipseButton.onClick.AddListener(() => {
            if(sunInstance == null)
            {
                sunInstance = Instantiate(sunPrefab, new Vector3(0,6.0f,15.0f), Quaternion.identity);
                sunInstance.transform.localScale = new Vector3(1.0f, 1.0f, 0.01f);
                sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;
            }
            if (moonInstance == null)
            {
                Vector3 spawnPosition = new Vector3(1.3f, 6f, 14.97f);
                moonInstance = Instantiate(moonPrefab, spawnPosition, Quaternion.identity);
            }
        });

        

        totalEclipseButton.onClick.AddListener(()=>SetEclipseMode(true));
        annularEclipseButton.onClick.AddListener(()=>SetEclipseMode(false));

        // 기존 리스너 제거
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        // 새로운 이벤트 리스너 추가
        leftButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        rightButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();

        // PointerDown/Up 이벤트 추가
        EventTrigger leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();

        // 왼쪽 버튼 이벤트
        EventTrigger.Entry leftPointerDown = new EventTrigger.Entry();
        leftPointerDown.eventID = EventTriggerType.PointerDown;
        leftPointerDown.callback.AddListener((data) => { isLeftButtonPressed = true; });
        leftTrigger.triggers.Add(leftPointerDown);

        EventTrigger.Entry leftPointerUp = new EventTrigger.Entry();
        leftPointerUp.eventID = EventTriggerType.PointerUp;
        leftPointerUp.callback.AddListener((data) => { isLeftButtonPressed = false; });
        leftTrigger.triggers.Add(leftPointerUp);

        // 오른쪽 버튼 이벤트
        EventTrigger.Entry rightPointerDown = new EventTrigger.Entry();
        rightPointerDown.eventID = EventTriggerType.PointerDown;
        rightPointerDown.callback.AddListener((data) => { isRightButtonPressed = true; });
        rightTrigger.triggers.Add(rightPointerDown);

        EventTrigger.Entry rightPointerUp = new EventTrigger.Entry();
        rightPointerUp.eventID = EventTriggerType.PointerUp;
        rightPointerUp.callback.AddListener((data) => { isRightButtonPressed = false; });
        rightTrigger.triggers.Add(rightPointerUp);
    }

    void Update()
    {
        if(sunInstance == null) return;
        // Touch touch = Input.GetTouch(0);

        // if(touch.phase == TouchPhase.Began) 
        // {
        //     if (moonInstance == null)
        //     {
        //         // 초기 터치한 곳에 달 프리펩 생성
        //         Vector3 spawnPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, 6.0f, sunInstance.transform.position.z + moonFixedOffset));
        //         moonInstance = Instantiate(moonPrefab, spawnPosition, Quaternion.identity);

        //         Vector3 newScale = moonInstance.transform.localScale;
        //         newScale.x = isTotalEclipse ? sunInstance.transform.localScale.x : sunInstance.transform.localScale.x * 0.9f;
        //         newScale.y = isTotalEclipse ? sunInstance.transform.localScale.y : sunInstance.transform.localScale.y * 0.9f;
        //         moonInstance.transform.localScale = newScale;

        //         moonZPos = moonInstance.transform.position.z;
        //         touched = true;
        //     }
        //     else
        //     {
        //         // 달이 이미 존재하고 그 달을 터치하고 있으면 touched를 true로 세팅
        //         Ray ray = arCamera.ScreenPointToRay(touch.position);
        //         RaycastHit hit;    

        //         if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == moonInstance)
        //         {
        //             Vector3 touchWorldPosition = arCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, moonZPos));
        //             touchOffset = moonInstance.transform.position - touchWorldPosition;
        //             touched = true;
        //         }
        //     }
        // }
        // if(touch.phase == TouchPhase.Ended)
        // {
        //     touched = false;
        // }
        // if (touched && moonInstance != null)
        // {
        //     // 달을 드래그해서 움직임
        //     Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, moonZPos);
        //     Vector3 newPosition = arCamera.ScreenToWorldPoint(touchPosition) + touchOffset;
        //     newPosition.z = moonZPos; // Lock z position
        //     moonInstance.transform.position = newPosition;
        // }

        AdjustBrightness();
        sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;

        // 버튼 상태에 따른 지속적인 이동
        if (moonInstance != null)
        {
            Vector3 newPosition = moonInstance.transform.position;
            float previousX = newPosition.x;
            Vector3 scale = sunGlow.transform.localScale;

            if (isLeftButtonPressed)
            {
                newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - MOVE_DISTANCE);
                moonInstance.transform.position = newPosition;
            }
            if (isRightButtonPressed)
            {
                newPosition.x = Mathf.Min(MAX_X_RANGE, newPosition.x + MOVE_DISTANCE);
                moonInstance.transform.position = newPosition;
            }

            // Sun Glow 스케일 조정
            // if (Mathf.Abs(newPosition.x) <= 1.0f && Mathf.Abs(newPosition.x) >= 0.0f)
            // {
            //     // 0으로 가까워질수록 스케일 감소
            //     float targetScale = Mathf.Lerp(MIN_SCALE, MAX_SCALE, Mathf.Abs(newPosition.x));
            //     scale.x = Mathf.Lerp(scale.x, targetScale, 0.1f);
            //     scale.y = Mathf.Lerp(scale.y, targetScale, 0.1f);
            //     sunGlow.transform.localScale = scale;
                
            //     Debug.Log($"Scale: {scale.x}, Position: {newPosition.x}");
            // }

            if (Mathf.Abs(newPosition.x) <= 1.0f)
            {
                // 현재 스케일 가져오기
                Vector3 currentScale = sunGlow.transform.localScale;
                
                // |x| 값이 0에 가까울수록 MIN_SCALE에 가까워지고, 1에 가까울수록 MAX_SCALE에 가까워짐
                float targetScale = Mathf.Lerp(MIN_SCALE, MAX_SCALE, Mathf.Abs(newPosition.x));

                Vector3 newScale = new Vector3(
                    Mathf.Lerp(currentScale.x, targetScale, 0.1f),
                    Mathf.Lerp(currentScale.y, targetScale, 0.1f),
                    currentScale.z
                );
                
                sunGlow.transform.localScale = newScale;
            }
        }
    }

    private void SetEclipseMode(bool isTotal)
    {
        isTotalEclipse = isTotal;
        
        
        if(moonInstance != null)
        {
            // 달 크기 조절
            Vector3 newScale = moonInstance.transform.localScale;
            newScale.x = isTotalEclipse ? sunInstance.transform.localScale.x : sunInstance.transform.localScale.x * 0.9f;
            newScale.y = isTotalEclipse ? sunInstance.transform.localScale.y : sunInstance.transform.localScale.y * 0.9f;
            moonInstance.transform.localScale = newScale;
        }    
    }

    private void AdjustBrightness()
    {
        if (moonInstance == null || sunInstance == null || colorGrading == null)
            return;

        // 태양-달, 태양-카메라 거리 계산
        Vector3 sunToMoon = moonInstance.transform.position - sunInstance.transform.position;
        Vector3 sunToCamera = arCamera.transform.position - sunInstance.transform.position;

        // sunToMoon을 sunToCamera에 투영
        float projectionLength = Vector3.Dot(sunToMoon, sunToCamera.normalized);
        Vector3 projectionPoint = sunInstance.transform.position + sunToCamera.normalized * projectionLength;

        // 달에서 투영 포인트까지 거리
        float distanceFromLine = Vector3.Distance(moonInstance.transform.position, projectionPoint);

        // 달이 태양의 지름 안쪽에 들어오면 일식 진행
        if (projectionLength > 0 && distanceFromLine <= sunDiameter)
        {
            float eclipseFactor = Mathf.Clamp01(1 - (distanceFromLine / sunDiameter));

            colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, eclipseFactor);
            colorGrading.saturation.value = Mathf.Lerp(0f, -25f, eclipseFactor);

        }
        else
        {
            // 태양 범위를 벗어나면 밝기 리셋
            colorGrading.postExposure.value = 0f;
            colorGrading.saturation.value = 0f;
        }

    }
}
