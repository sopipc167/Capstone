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
    private GameObject sunMaskPrefab;
    [SerializeField]
    private GameObject moonMaskPrefab;
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
    private Button SolarEclipseButton;
    [SerializeField]
    private Button LunarEclipseButton;
    [SerializeField]
    private Toggle autoMoveToggle;
    private bool isAutoMoving = false;
    private const float AUTO_MOVE_DISTANCE = 0.001f;
    private const float AUTO_MOVE_DISTANCE_Y = 0.0004f;

    private const float MOVE_DISTANCE = 0.02f;
    private const float MOVE_Y_DISTANCE = 0.0075f;
    private const float MAX_X_RANGE = 1.3f;  // 최대 X 좌표
    private const float MIN_X_RANGE = -1.3f; // 최소 X 좌표
    private const float MAX_Y_RANGE = 6.0f;  // 최대 Y 좌표
    private const float MIN_Y_RANGE = 5.5f;  // 최소 Y 좌표
    private const float MIN_SCALE = 0.4f;   // Sun Glow 최소 스케일
    private const float MAX_SCALE = 1.0f;    // Sun Glow 최대 스케일

    private GameObject sunInstance;
    private GameObject moonInstance;
    private GameObject maskInstance;
    
    private bool touched = false;
    private ColorGrading colorGrading;


    private float sunDiameter = 0.5f;
    private float moonDiameter = 0.25f;
    private float moonFixedOffset = -0.03f;
    private float moonZPos;
    private Vector3 touchOffset;

    bool isTotalEclipse = true;

    private bool isLeftButtonPressed = false;
    private bool isRightButtonPressed = false;
    private bool isMovingUp = true;

    private enum EclipseType
    {
        None,
        Solar,
        Lunar
    }
    private EclipseType eclipseType = EclipseType.None;

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


        SolarEclipseButton.onClick.AddListener(() => {
            eclipseType = EclipseType.Solar;
            if(sunInstance == null)
            {
                sunInstance = Instantiate(sunPrefab, new Vector3(0,6.0f,15.0f), Quaternion.identity);
                sunInstance.transform.localScale = new Vector3(1.0f, 1.0f, 0.01f);
                sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;
            }
            if (maskInstance == null)
            {
                Vector3 spawnPosition = new Vector3(1.3f, 5.5f, 14.97f);
                maskInstance = Instantiate(sunMaskPrefab, spawnPosition, Quaternion.identity);
            }
        });

        LunarEclipseButton.onClick.AddListener(() => {
            eclipseType = EclipseType.Lunar;
            if(moonInstance == null)
            {
                moonInstance = Instantiate(moonPrefab, new Vector3(0f, 6f, 15.0f), Quaternion.identity);
                moonInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f);
            }
            if(maskInstance == null)
            {
                Vector3 spawnPosition = new Vector3(1.3f, 5.5f, 14.97f);
                maskInstance = Instantiate(moonMaskPrefab, spawnPosition, Quaternion.identity);
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
        // 일식 효과
        if(eclipseType == EclipseType.Solar)
        {
            if(sunInstance == null || maskInstance == null) return;

            AdjustBrightness();
            sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;


            // 버튼 상태에 따른 지속적인 이동
            if (maskInstance != null)
            {
                Vector3 newPosition = maskInstance.transform.position;
                float previousX = newPosition.x;
                Vector3 scale = sunGlow.transform.localScale;

                if (isLeftButtonPressed)
                {
                    newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - MOVE_DISTANCE);

                    if(newPosition.x < 0)
                    {
                        newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                    }
                    else
                    {
                        newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                    }
                    
                    maskInstance.transform.position = newPosition;
                    
                }
                if (isRightButtonPressed)
                {
                    newPosition.x = Mathf.Min(MAX_X_RANGE, newPosition.x + MOVE_DISTANCE);
                    // x가 0보다 작으면 y 감소, 크면 y 증가
                    if (newPosition.x < 0)
                    {
                        newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                    }
                    else
                    {
                        newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                    }
                    maskInstance.transform.position = newPosition;
                }

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

            if(maskInstance != null && autoMoveToggle.isOn)
            {
                Vector3 newPosition = maskInstance.transform.position;
                newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - AUTO_MOVE_DISTANCE);
                maskInstance.transform.position = newPosition;
            }
        }
        // 월식 효과
        else if(eclipseType == EclipseType.Lunar)
        {
            if(moonInstance == null || maskInstance == null) return;

            

            // 버튼 상태에 따른 지속적인 이동
            if (maskInstance != null)
            {
                Vector3 newPosition = maskInstance.transform.position;

                if (isLeftButtonPressed)
                {
                    newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - MOVE_DISTANCE);

                    if(newPosition.x < 0)
                    {
                        newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                    }
                    else
                    {
                        newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                    }
                    
                    maskInstance.transform.position = newPosition;
                    
                }
                if (isRightButtonPressed)
                {
                    newPosition.x = Mathf.Min(MAX_X_RANGE, newPosition.x + MOVE_DISTANCE);
                    // x가 0보다 작으면 y 감소, 크면 y 증가
                    if (newPosition.x < 0)
                    {
                        newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                    }
                    else
                    {
                        newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                    }
                    maskInstance.transform.position = newPosition;
                }

                if (Mathf.Abs(newPosition.x) <= 0.5f)
                {
                    Renderer moonRenderer = moonInstance.GetComponent<Renderer>();
                    if (moonRenderer != null)
                    {
                        Color originalColor = Color.white; // 원래 색상
                        Color targetColor = Color.red;    // 목표 색상 (빨간색)
                        
                        // |x| 값이 0에 가까울수록 빨간색, 1에 가까울수록 원래 색상
                        Color lerpedColor = Color.Lerp(targetColor, originalColor, Mathf.Abs(newPosition.x));
                        
                        // 현재 색에서 계산된 색상으로 부드럽게 전환
                        Color currentColor = moonRenderer.material.color;
                        Color newColor = Color.Lerp(currentColor, lerpedColor, 0.01f);
                        
                        moonRenderer.material.color = newColor;
                    }
                } 
                else 
                {
                    Renderer moonRenderer = moonInstance.GetComponent<Renderer>();
                    Color currentColor = moonRenderer.material.color;
                    Color originalColor = Color.Lerp(currentColor, Color.white, 0.01f);
                    moonRenderer.material.color = originalColor;
                }
            }

            if(maskInstance != null && autoMoveToggle.isOn)
            {
                Vector3 newPosition = maskInstance.transform.position;
                newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - AUTO_MOVE_DISTANCE);
                if(newPosition.x < 0)
                {
                    newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - AUTO_MOVE_DISTANCE_Y);
                }
                else
                {
                    newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + AUTO_MOVE_DISTANCE_Y);
                }
                maskInstance.transform.position = newPosition;
            }
        }
    }

    private void SetEclipseMode(bool isTotal)
    {
        isTotalEclipse = isTotal;
        
        
        if(maskInstance != null)
        {
            // 달 크기 조절
            Vector3 newScale = maskInstance.transform.localScale;
            newScale.x = isTotalEclipse ? sunInstance.transform.localScale.x : sunInstance.transform.localScale.x * 0.9f;
            newScale.y = isTotalEclipse ? sunInstance.transform.localScale.y : sunInstance.transform.localScale.y * 0.9f;
            maskInstance.transform.localScale = newScale;
        }    
    }

    private void AdjustBrightness()
    {
        if (maskInstance == null || sunInstance == null || colorGrading == null)
            return;

        // 태양-달, 태양-카메라 거리 계산
        Vector3 sunToMoon = maskInstance.transform.position - sunInstance.transform.position;
        Vector3 sunToCamera = arCamera.transform.position - sunInstance.transform.position;

        // sunToMoon을 sunToCamera에 투영
        float projectionLength = Vector3.Dot(sunToMoon, sunToCamera.normalized);
        Vector3 projectionPoint = sunInstance.transform.position + sunToCamera.normalized * projectionLength;

        // 달에서 투영 포인트까지 거리
        float distanceFromLine = Vector3.Distance(maskInstance.transform.position, projectionPoint);

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

    private void LunarEclipseEffect()
    {
        if (maskInstance == null || moonInstance == null || colorGrading == null)
            return;

        // 태양-달, 태양-카메라 거리 계산
        Vector3 sunToMoon = maskInstance.transform.position - moonInstance.transform.position;
        Vector3 sunToCamera = arCamera.transform.position - moonInstance.transform.position;

        // sunToMoon을 sunToCamera에 투영
        float projectionLength = Vector3.Dot(sunToMoon, sunToCamera.normalized);
        Vector3 projectionPoint = moonInstance.transform.position + sunToCamera.normalized * projectionLength;

        // 달에서 투영 포인트까지 거리
        float distanceFromLine = Vector3.Distance(maskInstance.transform.position, projectionPoint);

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
