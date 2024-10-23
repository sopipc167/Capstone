using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateOnPlane : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private LayerMask placedObjectLayerMask;
    private Vector2 touchPosition;
    private Ray ray;
    private RaycastHit hit;

    private void Update()
    {
        // 화면을 터치하지 않고 있거나 첫번째 터치의 상태가 Began이 아니면 실행하지 않는다
        if(!TouchUtility.TryGetInputPosition(out touchPosition))
        {
            if(Input.touchCount == 2)   // 줌 기능
            {
                PlacedObject.SelectedObject.sizeUpdate(Input.GetTouch(0), Input.GetTouch(1));
            }
            return;
        }
        
        // 오브젝트 선택
        ray = arCamera.ScreenPointToRay(touchPosition);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, placedObjectLayerMask))
        {
            PlacedObject.SelectedObject = hit.transform.GetComponentInChildren<PlacedObject>();

            return;
        }

        // 오브젝트 선택이 아닐 경우 오브젝트 선택 취소
        PlacedObject.SelectedObject = null;

        // 오브젝트 생성
        if(TouchUtility.Raycast(touchPosition, out Pose hitPose))
        {
            Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
        }
    }
}
