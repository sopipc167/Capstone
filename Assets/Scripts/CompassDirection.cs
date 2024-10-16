using UnityEngine;

public class CompassDirection : MonoBehaviour
{
    public LineRenderer lineRenderer;  // 선을 그리기 위한 LineRenderer
    public float lineLength = 5.0f;    // 선의 길이 설정
    public 

    void Start()
    {
        // 나침반 활성화
        Input.compass.enabled = true;

        // LineRenderer 설정
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;  // 선의 시작 너비
            lineRenderer.endWidth = 0.1f;    // 선의 끝 너비
            lineRenderer.positionCount = 2;  // 두 점으로 선을 그릴 예정
        }
    }

    void Update()
    {
        float heading = 0;
        // 기기의 방위각 받아옴
        void ReceiveNorth(float N)
        {
            heading = N;
        }

        // LineRenderer를 통해 정북쪽을 가리키는 선을 그림
        Vector3 startPosition = transform.position;
        Vector3 northDirection = Quaternion.Euler(0, -heading, 0) * Vector3.forward;

        // 선의 시작점과 끝점 설정
        lineRenderer.SetPosition(0, startPosition);  // 시작점
        lineRenderer.SetPosition(1, startPosition + northDirection * lineLength);  // 끝점
    }
}