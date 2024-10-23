using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlacedObject : MonoBehaviour
{
    [SerializeField]
    private GameObject sphereSelected;
    
    private float initialDistance;
    private Vector3 initialScale;

    public bool IsSelected
    {
        get => SelectedObject == this;
    }

    private static PlacedObject selectedObject;
    public static PlacedObject SelectedObject
    {
        get => selectedObject;
        set
        {
            // 이미 선택된 오브젝트를 선택했을 때는 프로퍼티 종료
            if(selectedObject == value)
            {
                return;
            }

            // 기존에 선택된 다른 오브젝트가 있을 경우
            // 해당 오브젝트의 sphereSelected 오브젝트 비활성화
            if(selectedObject != null)
            {
                selectedObject.sphereSelected.SetActive(false);
            }

            // selectedObject에 value값을 저장하고
            selectedObject = value;
            // 현재 선택된 오브젝트의 sphereSelected 오브젝트 활성화
            if(value != null)
            {
                value.sphereSelected.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        sphereSelected.SetActive(false);
    }

    public void OnPointerDrag(BaseEventData bed)
    {
        if(IsSelected)
        {
            PointerEventData ped = (PointerEventData) bed;
            if(TouchUtility.Raycast(ped.position, out Pose hitPose))
            {
                transform.position = hitPose.position;
            }
        }
    }

    public void sizeUpdate(Touch touchZero, Touch touchOne)
    {
        if(touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
        {
            return;
        }

        if(touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
        {
            initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
            initialScale = transform.localScale;
        }
        else
        {
            var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

            if(Mathf.Approximately(initialDistance, 0))
            {
                return;
            }

            var factor = currentDistance / initialDistance;
            transform.localScale = initialScale * factor;
        }
    }
}
