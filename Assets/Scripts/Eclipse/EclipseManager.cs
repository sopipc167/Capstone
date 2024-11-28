using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using System;

public class EclipseManager : MonoBehaviour
{
    /* Manager */
    private XROrigin xrOrigin;
    private DataManager dataManager;
    private CompassManager compassManager;
    private bool isInitialized = false;
    private float trueNorth = 0.0f;

    /* 카메라 */
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private ARCameraManager ARCameraManager;

    /* 오브젝트 */
    [SerializeField]
    private GameObject moonPrefab;
    [SerializeField]
    private GameObject sunPrefab;
    private GameObject sunGlow;
    [SerializeField]
    private GameObject realSunPrefab;
    [SerializeField]
    private GameObject sunMaskPrefab;
    [SerializeField]
    private GameObject moonMaskPrefab;
    [SerializeField]
    private PostProcessVolume postProcessVolume;
    private GameObject realSun;

    /* UI */
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
    private Button RealSolarEclipseButton;
    [SerializeField]
    private Button LunarEclipseButton;
    [SerializeField]
    private Toggle autoMoveToggle;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Toggle realTimeToggle;

    /* 상수 */
    private const float AUTO_MOVE_DISTANCE = 0.005f;
    private const float AUTO_MOVE_DISTANCE_Y = 0.0012f;
    private const float MOVE_DISTANCE = 0.02f;
    private const float MOVE_Y_DISTANCE = 0.0046f;
    private const float MAX_X_RANGE = 1.3f;  // 최대 X 좌표
    private const float MIN_X_RANGE = -1.3f; // 최소 X 좌표
    private const float MAX_Y_RANGE = 6.0f;  // 최대 Y 좌표
    private const float MIN_Y_RANGE = 5.7f;  // 최소 Y 좌표
    private const float MIN_SCALE = 0.4f;   // Sun Glow 최소 스케일
    private const float MAX_SCALE = 1.0f;    // Sun Glow 최대 스케일
    float radius = 50.0f;

    /* 오브젝트 인스턴스 */
    private GameObject sunInstance;
    private GameObject moonInstance;
    private GameObject maskInstance;
    private ColorGrading colorGrading;

    /* 일식 관련 변수 */
    private float realSunDiameter = 2.0f;
    private float sunDiameter = 0.5f;
    bool isTotalEclipse = true;

    /* 버튼 변수 */
    private bool isLeftButtonPressed = false;
    private bool isRightButtonPressed = false;
    private float currentOffset = 0.0f;

    private enum EclipseType
    {
        None,
        Solar,
        Lunar
    }
    private EclipseType eclipseType = EclipseType.None;

    void Start()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        dataManager = FindAnyObjectByType<DataManager>();
        compassManager = FindAnyObjectByType<CompassManager>();
        realSun = Instantiate(realSunPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        
        postProcessVolume.profile.TryGetSettings(out colorGrading);

        ARCameraManager.frameReceived += FrameLightUpdated;

        backButton.onClick.AddListener(ResetToInitialState);


        SolarEclipseButton.onClick.AddListener(() => {
            eclipseType = EclipseType.Solar;
            if(sunInstance == null)
            {
                //Vector3 sunPosition = arCamera.transform.position + arCamera.transform.forward * 15.0f + arCamera.transform.up * 6.0f;
                sunInstance = Instantiate(sunPrefab, new Vector3(0f, 6f, 15.0f), Quaternion.identity);
                sunInstance.transform.localScale = new Vector3(1.0f, 1.0f, 0.01f);
                sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;
            }
            if (maskInstance == null)
            {
                //Vector3 maskPosition = arCamera.transform.position + arCamera.transform.forward * 14.97f + arCamera.transform.right * 1.3f + arCamera.transform.up * 5.7f;
                //Vector3 spawnPosition = new Vector3(arCamera.transform.position.x + 1.3f, arCamera.transform.position.y + 5.7f, arCamera.transform.position.z + 14.97f);
                maskInstance = Instantiate(sunMaskPrefab, new Vector3(1.3f, 5.7f, 14.97f), Quaternion.identity);
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
                Vector3 spawnPosition = new Vector3(1.3f, 5.7f, 14.97f);
                maskInstance = Instantiate(moonMaskPrefab, spawnPosition, Quaternion.identity);
            }
        });

        RealSolarEclipseButton.onClick.AddListener(() => {
            eclipseType = EclipseType.Solar;
            // maskInstance가 없을 때만 생성
            if(maskInstance == null && realSun != null)
            {
                // realSun의 위치 가져오기
                Vector3 realSunPosition = realSun.transform.position;
                
                // 카메라에서 realSun까지의 방향 벡터
                Vector3 directionToSun = (realSunPosition - arCamera.transform.position).normalized;

                // 위쪽 방향과 태양 방향의 외적으로 오른쪽 방향 계산
                Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToSun).normalized;
                
                // realSun보다 약간 앞에 위치하도록 설정 (z축으로 약간 앞으로)
                float distanceFromCamera = Vector3.Distance(arCamera.transform.position, realSunPosition);
                Vector3 maskPosition = arCamera.transform.position + directionToSun * (distanceFromCamera - 2.0f);

                float rightOffset = 200.0f;
                maskPosition += rightDirection * rightOffset;
                // 마스크 생성
                maskInstance = Instantiate(sunMaskPrefab, maskPosition, arCamera.transform.rotation);
                maskInstance.transform.localScale = new Vector3(4, 4, 0);
            }

            // // maskInstance가 이미 존재하는 경우, realSun 앞에 위치하도록 업데이트
            // if(maskInstance != null && realSun != null)
            // {
            //     Vector3 realSunPosition = realSun.transform.position;
            //     Vector3 directionToSun = (realSunPosition - arCamera.transform.position).normalized;
            //     float distanceFromCamera = Vector3.Distance(arCamera.transform.position, realSunPosition);
                
            //     maskInstance.transform.position = arCamera.transform.position + directionToSun * (distanceFromCamera - 0.03f);
            //     maskInstance.transform.rotation = arCamera.transform.rotation;
            // }
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

    private void ResetToInitialState()
    {
        // 기존 오브젝트들 제거
        if(sunInstance != null) 
            Destroy(sunInstance);
        if(moonInstance != null) 
            Destroy(moonInstance);
        if(maskInstance != null) 
        {    
            Destroy(maskInstance);
            maskInstance = null;
        }

        // 변수들 초기화
        eclipseType = EclipseType.None;
        
        // 토글 상태 초기화
        if(autoMoveToggle != null)
            autoMoveToggle.isOn = false;

        // PostProcessing 효과 초기화
        if(colorGrading != null)
        {
            colorGrading.postExposure.value = 0f;
            colorGrading.temperature.value = 0f;
        }
    }

    void Update()
    {
        // 실제 태양으로 진행
        if(realTimeToggle.isOn)
        {
            realSun.SetActive(true);
            if (!isInitialized)
            {
                trueNorth = compassManager.GetTrueNorth();
                if (trueNorth == -1.0f) return;

                isInitialized = true;
            }

            if (isInitialized)
            {
                UpdateSolarObjects();
            }
            if(realSun == null || maskInstance == null) return;
            Shader.SetGlobalVector("_SunPosition", realSun.transform.position);
            Shader.SetGlobalVector("_MoonPosition", maskInstance.transform.position); 
            Shader.SetGlobalFloat("_SunRadius", 2.0f);
            Shader.SetGlobalFloat("_MoonRadius", 4.0f);

            if(maskInstance != null && realSun != null)
            {
                Vector3 realSunPosition = realSun.transform.position;
                Vector3 directionToSun = (realSunPosition - arCamera.transform.position).normalized;
                Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToSun).normalized;
                float distanceFromCamera = Vector3.Distance(arCamera.transform.position, realSunPosition);

                Vector3 basePosition = arCamera.transform.position + directionToSun * (distanceFromCamera - 0.03f);

                if(isLeftButtonPressed)
                {
                    currentOffset = Mathf.Max(-4.0f, currentOffset - 0.04f);
                }
                if(isRightButtonPressed)
                {
                    currentOffset = Mathf.Min(4.0f, currentOffset + 0.04f);
                }

                Vector3 newPosition = basePosition + rightDirection * currentOffset;
                newPosition.y = realSunPosition.y;

                maskInstance.transform.position = newPosition;
                //maskInstance.transform.rotation = arCamera.transform.rotation;
            }

            if(maskInstance != null && autoMoveToggle.isOn)
            {
                Vector3 realSunPosition = realSun.transform.position;
                Vector3 directionToSun = (realSunPosition - arCamera.transform.position).normalized;
                Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToSun).normalized;
                float distanceFromCamera = Vector3.Distance(arCamera.transform.position, realSunPosition);

                Vector3 basePosition = arCamera.transform.position + directionToSun * (distanceFromCamera - 0.03f);

                currentOffset = Mathf.Max(-4.0f, currentOffset - 0.01f);
                Vector3 newPosition = basePosition + rightDirection * currentOffset;
                newPosition.y = realSunPosition.y;

                maskInstance.transform.position = newPosition;
            }

            RealAdjustBrightness();
        }

        // 가상으로 진행
        if(!realTimeToggle.isOn)
        {
            realSun.SetActive(false);
            // 일식 효과
            if(eclipseType == EclipseType.Solar)
            {
                if(sunInstance == null || maskInstance == null) return;
                Shader.SetGlobalVector("_SunPosition", sunInstance.transform.position);
                Shader.SetGlobalVector("_MoonPosition", maskInstance.transform.position); 
                Shader.SetGlobalFloat("_SunRadius", 0.5f);
                Shader.SetGlobalFloat("_MoonRadius", 1.0f);

                //AdjustBrightness();
                sunGlow = sunInstance.transform.Find("Sun Glow").gameObject;

                sunInstance.transform.rotation = arCamera.transform.rotation;

                // 버튼 상태에 따른 지속적인 이동
                if (maskInstance != null)
                {
                    Vector3 newPosition = maskInstance.transform.position;
                    float newX = maskInstance.transform.position.x;
                    float newY = maskInstance.transform.position.y;
                    Vector3 sunPos = sunInstance.transform.position;

                    float previousX = newPosition.x;
                    Vector3 scale = sunGlow.transform.localScale;

                    if (isLeftButtonPressed)
                    {
                        newPosition.x = Mathf.Max(MIN_X_RANGE+sunPos.x, newPosition.x - MOVE_DISTANCE);

                        if(newPosition.x <= sunPos.x)
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
                        newPosition.x = Mathf.Min(MAX_X_RANGE+sunPos.x, newPosition.x + MOVE_DISTANCE);
                        // x가 0보다 작으면 y 감소, 크면 y 증가
                        if (newPosition.x <= sunPos.x)
                        {
                            newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                        }
                        else
                        {
                            newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                        }
                        maskInstance.transform.position = newPosition;
                    }

                    if (Mathf.Abs(newPosition.x) <= sunPos.x + 1.0f)
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
                    Vector3 sunPos = sunInstance.transform.position;
                    newPosition.x = Mathf.Max(MIN_X_RANGE+sunPos.x, newPosition.x - AUTO_MOVE_DISTANCE);
                    if(newPosition.x <= sunPos.x)
                    {
                        newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - AUTO_MOVE_DISTANCE_Y);
                    }
                    else
                    {
                        newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + AUTO_MOVE_DISTANCE_Y);
                    }
                    maskInstance.transform.position = newPosition;
                }

                AdjustBrightness();
            }
            // 월식 효과
            else if(eclipseType == EclipseType.Lunar)
            {
                if(moonInstance == null || maskInstance == null) return;

                Shader.SetGlobalVector("_SunPosition", moonInstance.transform.position);
                Shader.SetGlobalVector("_MoonPosition", maskInstance.transform.position); 
                Shader.SetGlobalFloat("_SunRadius", 0.5f);
                Shader.SetGlobalFloat("_MoonRadius", 1.0f);
                

                // 버튼 상태에 따른 지속적인 이동
                if (maskInstance != null)
                {
                    Vector3 newPosition = maskInstance.transform.position;

                    if (isLeftButtonPressed)
                    {
                        newPosition.x = Mathf.Max(MIN_X_RANGE, newPosition.x - MOVE_DISTANCE);

                        if(newPosition.x <= 0)
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
                        if (newPosition.x <= 0)
                        {
                            newPosition.y = Mathf.Min(MAX_Y_RANGE, newPosition.y + MOVE_Y_DISTANCE);
                        }
                        else
                        {
                            newPosition.y = Mathf.Max(MIN_Y_RANGE, newPosition.y - MOVE_Y_DISTANCE);
                        }
                        maskInstance.transform.position = newPosition;
                    }

                    if (Mathf.Abs(newPosition.x) <= 0.33f)
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
                            Color newColor = Color.Lerp(currentColor, lerpedColor, 0.1f);
                            
                            moonRenderer.material.color = newColor;
                        }
                    } 
                    else 
                    {
                        Renderer moonRenderer = moonInstance.GetComponent<Renderer>();
                        Color currentColor = moonRenderer.material.color;
                        Color originalColor = Color.Lerp(currentColor, Color.white, 0.1f);
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
    }

    public void UpdateSolarObjects()
    {
        CelestialObject obj;

        obj = dataManager.celestialObjects["sun"];
        UpdateStar(realSun, obj.alt, obj.az, radius);
    }

    public void UpdateStar(GameObject star, float altitude, float azimuth, float distance)
    {
        float adjustedAzimuth = azimuth - trueNorth;
        Vector3 starPosition = SphericalToCartesian(altitude, adjustedAzimuth, distance);
        //Vector3 starPosition = GetStarPosition(altitude, azimuth, distance, northDirection);

        star.transform.position = starPosition;
    }

    private void SetEclipseMode(bool isTotal)
    {
        isTotalEclipse = isTotal;
        
        
        if(maskInstance != null && sunInstance != null)
        {
            // 달 크기 조절
            Vector3 newScale = maskInstance.transform.localScale;
            newScale.x = isTotalEclipse ? sunInstance.transform.localScale.x : sunInstance.transform.localScale.x * 0.9f;
            newScale.y = isTotalEclipse ? sunInstance.transform.localScale.y : sunInstance.transform.localScale.y * 0.9f;
            maskInstance.transform.localScale = newScale;
        }
        if(maskInstance != null && realSun != null)
        {
            // 달 크기 조절
            Vector3 newScale = maskInstance.transform.localScale;
            newScale.x = isTotalEclipse ? realSun.transform.localScale.x : realSun.transform.localScale.x * 0.9f;
            newScale.y = isTotalEclipse ? realSun.transform.localScale.y : realSun.transform.localScale.y * 0.9f;
            maskInstance.transform.localScale = newScale;
        }   
    }

    public void FrameLightUpdated(ARCameraFrameEventArgs args)
    {
        if(eclipseType != EclipseType.Solar) return;

        var brightness = args.lightEstimation.averageBrightness;

        if(brightness.HasValue)
        {
            if(brightness.Value <= 0.25f)
            {
                colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, 2.5f, 0.5f*Time.deltaTime);
                colorGrading.saturation.value = Mathf.Lerp(colorGrading.saturation.value,25f,0.5f*Time.deltaTime);
                colorGrading.gamma.value = new Vector4(0.65f,0.65f,0.65f,0);
                AdjustBrightness(args);
            } 
            else
            {
                colorGrading.postExposure.value = 0f;
                colorGrading.saturation.value = 0f;
                colorGrading.gamma.value = new Vector4(1,1,1,0);
                AdjustBrightness(args);
            }
        }
    }

    private void AdjustBrightness(ARCameraFrameEventArgs args)
    {
        if (maskInstance == null || sunInstance == null || colorGrading == null)
            return;
        
        var brightness = args.lightEstimation.averageBrightness;

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

            if(brightness.Value <= 0.35f)
            {
                colorGrading.postExposure.value = Mathf.Lerp(2.5f, -5f, eclipseFactor);
                colorGrading.saturation.value = Mathf.Lerp(25f, -25f, eclipseFactor);
            }
            else
            {
                colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, eclipseFactor);
                colorGrading.saturation.value = Mathf.Lerp(0f, -25f, eclipseFactor);
            }

        }
        else
        {
            if(brightness.Value <= 0.35f)
            {
                colorGrading.postExposure.value = 2.5f;
                colorGrading.saturation.value = 25f;
                colorGrading.gamma.value = new Vector4(0.65f,0.65f,0.65f,0);
            }
            else
            {
                colorGrading.postExposure.value = 0f;
                colorGrading.saturation.value = 0f;
                colorGrading.gamma.value = new Vector4(1,1,1,0);
            }
            // // 태양 범위를 벗어나면 밝기 리셋
            // colorGrading.postExposure.value = 0f;
            // colorGrading.saturation.value = 0f;
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

    private void RealAdjustBrightness()
    {
        if (maskInstance == null || realSun == null || colorGrading == null)
        {   
            return;
        }
        

        // 태양-달, 태양-카메라 거리 계산
        Vector3 sunToMoon = maskInstance.transform.position - realSun.transform.position;
        Vector3 sunToCamera = arCamera.transform.position - realSun.transform.position;

        // sunToMoon을 sunToCamera에 투영
        float projectionLength = Vector3.Dot(sunToMoon, sunToCamera.normalized);
        Vector3 projectionPoint = realSun.transform.position + sunToCamera.normalized * projectionLength;

        // 달에서 투영 포인트까지 거리
        float distanceFromLine = Vector3.Distance(maskInstance.transform.position, projectionPoint);

        // 달이 태양의 지름 안쪽에 들어오면 일식 진행
        if (projectionLength > 0 && distanceFromLine <= realSunDiameter)
        {
            float eclipseFactor = Mathf.Clamp01(1 - (distanceFromLine / realSunDiameter));


            colorGrading.postExposure.value = Mathf.Lerp(0f, -5f, eclipseFactor);
            colorGrading.saturation.value = Mathf.Lerp(0f, -25f, eclipseFactor);
        }
        else
        {
            float eclipseFactor = Mathf.Clamp01(1 - (distanceFromLine / realSunDiameter));
            // 태양 범위를 벗어나면 밝기 리셋
            colorGrading.postExposure.value = Mathf.Lerp(colorGrading.postExposure.value, 0f, eclipseFactor);
            colorGrading.saturation.value = Mathf.Lerp(colorGrading.saturation.value, 0f, eclipseFactor);
        }

    }

    Vector3 SphericalToCartesian(float altitude, float azimuth, float radius)
    {
        float alt_radian = altitude * Mathf.Deg2Rad;
        float az_radian = azimuth * Mathf.Deg2Rad;

        float x = radius * Mathf.Cos(alt_radian) * Mathf.Sin(az_radian);
        float y = radius * Mathf.Sin(alt_radian);
        float z = radius * Mathf.Cos(alt_radian) * Mathf.Cos(az_radian);

        return new Vector3(x, y, z);
    }
}
