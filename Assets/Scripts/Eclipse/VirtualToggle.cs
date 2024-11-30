using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VirtualToggle : MonoBehaviour
{
    public Toggle toggle;
    public GameObject[] activeList;
    public GameObject[] inactiveList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject ui in inactiveList)
        {
            ui.SetActive(!toggle.isOn);
        }
        foreach (GameObject ui in activeList)
        {
            ui.SetActive(toggle.isOn);
        }
    }
}
