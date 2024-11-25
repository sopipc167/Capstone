using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VirtualToggle : MonoBehaviour
{
    public Toggle toggle;
    public GameObject[] uiList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject ui in uiList)
        {
            ui.SetActive(!toggle.isOn);
        }
    }
}
