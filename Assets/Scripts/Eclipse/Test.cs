using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Button solarEclipseButton;  // 일식 버튼
    [SerializeField]
    private RectTransform targetImage;  // 이미지의 RectTransform
    [SerializeField]
    private GameObject sunPrefab;  // 태양 프리팹
    
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        solarEclipseButton.onClick.AddListener(CreateSunAtImagePosition);
    }

    void Update()
    {
        
    }

    void CreateSunAtImagePosition()
    {
        // 이미지의 스크린 좌표 가져오기
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, targetImage.position);

        // 스크린 좌표를 월드 좌표로 변환
        Ray ray = mainCamera.ScreenPointToRay(screenPoint);
        float distanceFromCamera = 15f; // 카메라로부터의 거리
        Vector3 worldPosition = ray.GetPoint(distanceFromCamera);

        // 태양 오브젝트 생성
        if (sunPrefab != null)
        {
            Instantiate(sunPrefab, worldPosition, Quaternion.identity);
        }
    }
}
