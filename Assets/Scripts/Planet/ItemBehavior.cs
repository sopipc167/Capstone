using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Security.Policy;

public class ItemBehavior : MonoBehaviour
{

    [SerializeField]
    TMP_Text name;
    [SerializeField]
    RawImage img;
    public void SImg(string url)
    {
        StartCoroutine(SetImg(url));
    }
    public void SetName(string n)
    {
        name.text = n;
    }
    public IEnumerator SetImg(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        Debug.Log(url);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            img.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}
