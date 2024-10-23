using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TouchUtility : MonoBehaviour
{
    private static ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    static TouchUtility()
    {
        raycastManager = GameObject.FindObjectOfType<ARRaycastManager>();
    }

    // 충돌 확인
    public static bool Raycast(Vector2 screenPosition, out Pose pose)
    {
        if(raycastManager.Raycast(screenPosition, hits, TrackableType.AllTypes))
        {
            pose = hits[0].pose;
            return true;
        }
        else
        {
            pose = Pose.identity;
            return false;
        }
    }

    // 터치 중인지 확인
    public static bool TryGetInputPosition(out Vector2 position)
    {
        position = Vector2.zero;

        // 화면을 터치하지 않으면 false 반환
        if(Input.touchCount == 0)
        {
            return false;
        }

        // 첫 번째 터치의 위치 값을 position에 저장
        position= Input.GetTouch(0).position;

        // 첫 번째 터치의 상태가 Began이 아니면 false 반환
        if(Input.GetTouch(0).phase != TouchPhase.Began)
        {
            return false;
        }

        return true;
    }
}
