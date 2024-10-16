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

    public float shakeThreshold = 0.5f;  // ��鸲 ���� �Ӱ谪 (���� ����)
    public float calibrationCooldown = 5.0f;  // ���� ������ �ּ� ��� �ð� (��)

    private float lastShakeTime = 0f;  // ������ ��鸲 �ð�
    private Vector3 lastAcceleration;  // ���� ���ӵ� �� ����
    private bool isCalibrating = false;  // ���� ���� ������ ����

    //ī�޶� ����&������ ����
    float angleHor, angleVer;

    //������ ���� ����
    float heading;

    float longitude, latitude;
    public TextMeshProUGUI T_Alarm;
    void Start()
    {
        Input.location.Start();
        Input.gyro.enabled = true;
        Input.compass.enabled = true;
        lastAcceleration = Input.acceleration;

        //�ʱ� ī�޶� ���� ����
        transform.rotation = Quaternion.Euler(90, 0, 0);
        initialRotation = transform.rotation;

        //��ġ �޾ƿ���
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        // ��ġ�������� �۵� ���� ���� Ȯ��
        if (!Input.location.isEnabledByUser)
        {
            T_Alarm.text = "Location services are not enabled by the user.";
            yield break;
        }

        // ��ġ�������� ����
        Input.location.Start();

        // ���� ���� ���
        int maxWait = 20; // �ִ� ��� �ð�
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // �ð� ����
        if (maxWait <= 0)
        {
            T_Alarm.text = "Location services timed out.";
            yield break;
        }

        // ���� ����
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            T_Alarm.text = "Unable to determine device location.";
            yield break;
        }
        else
        {
            // ������ �浵 �޾ƿ���
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

        // ���̷� ���� �� �޾ƿ���
        Quaternion deviceRotation = Input.gyro.attitude;

        // ���� ȸ���� ���
        Quaternion adjustedRotation = new Quaternion(-deviceRotation.x, -deviceRotation.y, deviceRotation.z, deviceRotation.w);

        // ī�޶� ���� ����
        transform.rotation = initialRotation * adjustedRotation;

        //���� ī�޶� ���� ���� ����
        Quaternion deltaRotation = Quaternion.Inverse(initialRotation) * adjustedRotation;
        Vector3 deltaEulerAngles = deltaRotation.eulerAngles;

        //���̷μ����� ���� ���� ����
        angleVer = deltaEulerAngles.x;
        angleHor = deltaEulerAngles.y;

        //������ ���� [0,360]���� [-90,90]���� ����
        if (angleVer > 180) angleVer -= 360;

        //���� ���
        T_angles.text = "North : " + heading.ToString("f3") + "��" + "\nDirect : " + angleHor.ToString("f3") + "��" + "\nVer : " + angleVer.ToString("f3") + "��";
        T_Lon_Lat.text = "Long : " + longitude.ToString("f3") + "��" + "\nLat : " + latitude.ToString("f3") + "��";
    }
    void DetectShake()
    {
        // ���� ���ӵ� ���� �޾ƿ´�
        Vector3 acceleration = Input.acceleration;
        float shakeMagnitude = (acceleration - lastAcceleration).magnitude;

        // ���ӵ��� ��ȭ�� �Ӱ谪�� �Ѱ�, ���� ��� �ð��� ������ ��
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

            // ��ħ�� ������ �ʱ�ȭ
            Input.compass.enabled = false;
            Input.compass.enabled = true;

            // 2�� �� ���� �Ϸ� ǥ��
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
        // ��ħ���� ������(Heading)�� �޾ƿ� ȭ�鿡 ǥ��
        float heading = Input.compass.trueHeading;
    }

    public float GetNorth()
    {
        return heading;
    }
}