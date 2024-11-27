using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectorManager : MonoBehaviour
{
    public bool isRaycastActive = false; // Raycast Ȱ��ȭ ���¸� �����ϴ� �÷���
    private int layerMask; // Raycast�� ����� ���̾� ����ũ
    private RaycastHit hit; // Raycast �浹 ������ ����
    public DistanceManager DM; // ObjectManager ����
    public Button Confirm;
    private int hitCount = 0;
    private GameObject hitObject;

    void Start()
    {
        layerMask = LayerMask.GetMask("StarLayer"); // StarLayer�� ����
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
        // ȭ�� �߽ɿ��� Ray�� �߻�
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
