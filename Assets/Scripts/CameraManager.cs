using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ManageCamera : MonoBehaviour
{
    private Quaternion initialRotation;
    public TextMeshProUGUI T_angles;
    public TextMeshProUGUI T_Lon_Lat;

    public float shakeThreshold = 0.5f;  // 흔들림 감지 임계값 (조정 가능)
    public float calibrationCooldown = 5.0f;  // 보정 사이의 최소 대기 시간 (초)

    private float lastShakeTime = 0f;  // 마지막 흔들림 시간
    private Vector3 lastAcceleration;  // 이전 가속도 값 저장
    private bool isCalibrating = false;  // 현재 보정 중인지 여부

    //카메라 수평&수직각 저장
    float angleHor, angleVer;

    //정북쪽 각도 저장
    float heading;

    float longitude, latitude;
    public TextMeshProUGUI T_Alarm;
    void Start()
    {
        Input.location.Start();
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        lastAcceleration = Input.acceleration;

        //초기 카메라 조건 설정
        transform.rotation = Quaternion.Euler(90, 0, 0);
        initialRotation = transform.rotation;

        //위치 받아오기
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        // 위치정보서비스 작동 가능 여부 확인
        if (!Input.location.isEnabledByUser)
        {
            T_Alarm.text = "Location services are not enabled by the user.";
            yield break;
        }

        // 위치정보서비스 시작
        Input.location.Start();

        // 서비스 시작 대기
        int maxWait = 20; // 최대 대기 시간
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 시간 만료
        if (maxWait <= 0)
        {
            T_Alarm.text = "Location services timed out.";
            yield break;
        }

        // 연결 실패
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            T_Alarm.text = "Unable to determine device location.";
            yield break;
        }
        else
        {
            // 위도와 경도 받아오기
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Input.location.Stop();
                Application.Quit();
            }
        }

        DetectShake();
        UpdateCompass();

        if (Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
        }

        // 자이로 센서 값 받아오기
        Quaternion deviceRotation = Input.gyro.attitude;

        // 현재 회전값 계산
        Quaternion adjustedRotation = new Quaternion(-deviceRotation.x, -deviceRotation.y, deviceRotation.z, deviceRotation.w);

        // 카메라 각도 적용
        transform.rotation = initialRotation * adjustedRotation;

        //현재 카메라 각도 벡터 저장
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * adjustedRotation;
        Vector3 deltaEulerAngles = deltaRotation.eulerAngles;

        //자이로센서에 의한 각도 저장
        angleVer = deltaEulerAngles.x;
        angleHor = deltaEulerAngles.y;

        //수직각 범위 [0,360]에서 [-90,90]으로 조정
        if (angleVer > 180) angleVer -= 360;

        //각도 출력
        T_angles.text = "North : " + heading.ToString("f3") + "˚" + "\nDirect : " + angleHor.ToString("f3") + "˚" + "\nVer : " + angleVer.ToString("f3") + "˚";
        T_Lon_Lat.text = "Long : " + longitude.ToString("f3") + "˚" + "\nLat : " + latitude.ToString("f3") + "˚";
    }
    void DetectShake()
    {
        // 현재 가속도 값을 받아온다
        Vector3 acceleration = Input.acceleration;
        float shakeMagnitude = (acceleration - lastAcceleration).magnitude;

        // 가속도의 변화가 임계값을 넘고, 보정 대기 시간이 지났을 때
        if (shakeMagnitude > shakeThreshold && Time.time - lastShakeTime > calibrationCooldown)
        {
            lastShakeTime = Time.time;
            StartCalibration();
        }

        lastAcceleration = acceleration;
    }
    void StartCalibration()
    {
        if (!isCalibrating)
        {
            isCalibrating = true;
            T_Alarm.text = "Calibration started: Shake detected!";

            // 나침반 데이터 초기화
            Input.compass.enabled = false;
            Input.compass.enabled = true;

            // 2초 후 보정 완료 표시
            Invoke(nameof(FinishCalibration), 2.0f);
        }
    }
    void FinishCalibration()
    {
        isCalibrating = false;
        T_Alarm.text = "Calibration finished.";
        SendMessage("ReceiveNorth", heading);
    }

    void UpdateCompass()
    {
        // 나침반의 방위각(Heading)을 받아와 화면에 표시
        float heading = Input.compass.trueHeading;
    }

    public float GetNorth()
    {
        return heading;
    }
}