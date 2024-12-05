using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Info_PanelControler : MonoBehaviour
{
    private JSON_Parser parser;
    
    Star_data data;
    [SerializeField]
    TMP_Text name;
    [SerializeField]
    TMP_Text flux_v;
    [SerializeField]
    TMP_Text ra;
    [SerializeField]
    TMP_Text radius;
    [SerializeField]
    TMP_Text distance;
    [SerializeField]
    TMP_Text dec;
    Star_data tmp;
    public void getData(string json)
    {
        parser = new JSON_Parser();
        tmp = parser.readJSON(json);
        setData(tmp);
    }
    public void setData(Star_data d)
    {
        this.data = d;
        Debug.Log(data);
        Debug.Log(d);
        name.text = data.nameUnicode;
        flux_v.text = data.flux_v;
        ra.text = data.ra;
        radius.text = data.radius + " km";
        distance.text = data.distance + " AU";
        dec.text = data.dec;
    }
}