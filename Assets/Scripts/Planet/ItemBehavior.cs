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
    Button bt;
    GameObject panel;
    GameObject info;
    Star_data dt;
    private void Start()
    {
        panel = GameObject.Find("Canvas");
        info = panel.transform.Find("Info").gameObject;
        panel= panel.transform.Find("Search").gameObject;
        
        bt = this.gameObject.GetComponent<Button>();
        bt.onClick.AddListener(btn);
    }
    public void btn()
    {
        info.SetActive(true);
        info.GetComponent<Info_PanelControler>().setData(dt);
        panel.SetActive(false);
        this.gameObject.SetActive(false);
    }
    public void setData(Star_data d)
    {
        this.dt = d;
    }
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
