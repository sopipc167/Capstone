using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//�̱������� �Ľ� Ŭ���� �ۼ�
[Serializable]
public class JSON_Parser
{
    private TextAsset json_Data;
    private static JSON_Parser parser;

    public static JSON_Parser instance
    {
        get
        {
            if (parser == null)
            {
                parser = new JSON_Parser();
            }
            return parser;
        }
    }

    public Star_data readJSON(string data)
    {
        Star_data tmp = JsonUtility.FromJson<Star_data>(data);
        return tmp;
    }
    private void OnDestroy()
    {
        parser = null;
    }
}
[Serializable]
public class Star_data
{
    public string name;
    public string nameUnicode;
    public string img;
    public string lat;
    public string lon;
    public string radius;
    public string ra;
    public string flux_v;
    public float distance;
    public string dec;
}