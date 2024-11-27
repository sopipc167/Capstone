using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DistanceManager : MonoBehaviour
{

    //천체에 콜리더 추가를 위한 변수
    public Button DistanceCalculate;
    public ObjectManager obj;
    public DetectorManager detectorManager;
    public CalculateDistance calculator;
    private Dictionary<GameObject, Star> selected = new Dictionary<GameObject, Star>();
    //public 

    // Start is called before the first frame update

    void Start()
    {
        // 버튼 클릭 이벤트 등록
        DistanceCalculate.enabled = true;
        DistanceCalculate.onClick.AddListener(ActivateColliders);
        detectorManager.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ActivateColliders()
    {
        DistanceCalculate.enabled = false;
        foreach (Star star in obj.GetAllStars())
        {
            star.collider.enabled = true;
        }
        detectorManager.enabled = true;
    }

    void DeactivateColliders()
    {
        selected.Clear();
        foreach (Star star in obj.GetAllStars())
        {
            star.collider.enabled = false;
        }
        detectorManager.enabled = false;
    }
    public void addSelected(GameObject s)
    {
        selected[s] = s.GetComponent<Star>();
    }
    public List<Star> GetSelectedStars()
    {
        return new List<Star>(selected.Values);
    }
}
