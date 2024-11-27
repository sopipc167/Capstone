using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectorManager : MonoBehaviour
{
    public bool isRaycastActive = false; // Raycast 활성화 상태를 관리하는 플래그
    private int layerMask; // Raycast에 사용할 레이어 마스크
    private RaycastHit hit; // Raycast 충돌 정보를 저장
    public DistanceManager DM; // ObjectManager 참조
    public Button Confirm;
    private int hitCount = 0;
    private GameObject hitObject;

    void Start()
    {
        layerMask = LayerMask.GetMask("StarLayer"); // StarLayer만 감지
        Confirm.onClick.AddListener(selectStar);
        Confirm.enabled = false;
    }

    void Update()
    {
        if (hitCount == 2)
        {
            hitCount = 0;
            ToggleRaycast();
        }
    }

    public void PerformRaycast()
    {
        // 화면 중심에서 Ray를 발사
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            hitObject = hit.collider.gameObject;
            Confirm.enabled = true;

            Debug.Log($"Raycast hit: {hitObject.name}");
        }
        else
        {
            Confirm.enabled = false;
        }
    }

    public void ToggleRaycast()
    {
        isRaycastActive = !isRaycastActive;
        Debug.Log($"Raycast is now {(isRaycastActive ? "Active" : "Inactive")}");
    }

    private void selectStar()
    {
        DM.addSelected(hitObject);
        hitCount++;
    }
}
