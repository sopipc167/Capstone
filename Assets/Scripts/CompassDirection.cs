using UnityEngine;

public class CompassDirection : MonoBehaviour
{
    public LineRenderer lineRenderer;  // ���� �׸��� ���� LineRenderer
    public float lineLength = 5.0f;    // ���� ���� ����
    public 

    void Start()
    {
        // ��ħ�� Ȱ��ȭ
        Input.compass.enabled = true;

        // LineRenderer ����
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;  // ���� ���� �ʺ�
            lineRenderer.endWidth = 0.1f;    // ���� �� �ʺ�
            lineRenderer.positionCount = 2;  // �� ������ ���� �׸� ����
        }
    }

    void Update()
    {
        float heading = 0;
        // ����� ������ �޾ƿ�
        void ReceiveNorth(float N)
        {
            heading = N;
        }

        // LineRenderer�� ���� �������� ����Ű�� ���� �׸�
        Vector3 startPosition = transform.position;
        Vector3 northDirection = Quaternion.Euler(0, -heading, 0) * Vector3.forward;

        // ���� �������� ���� ����
        lineRenderer.SetPosition(0, startPosition);  // ������
        lineRenderer.SetPosition(1, startPosition + northDirection * lineLength);  // ����
    }
}