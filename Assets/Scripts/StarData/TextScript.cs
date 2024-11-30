using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public TextMeshPro text;

    public void SetText(string content)
    {
        text.text = content;
    }

    void Update()
    {
        text.transform.LookAt(Camera.main.transform);
        text.transform.Rotate(0, 180, 0);
    }
}
