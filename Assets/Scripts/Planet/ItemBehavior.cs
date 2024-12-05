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
    GameObject obj_manager;
    Star_data dt;
    private void Start()
    {
        panel = GameObject.Find("Canvas");
        obj_manager = GameObject.Find("StarManager").gameObject;
        panel= panel.transform.Find("Search").gameObject;
        
        bt = this.gameObject.GetComponent<Button>();
        bt.onClick.AddListener(btn);
    }
    public void btn()
    {
        obj_manager.GetComponent<ObjectManager>().setTrackingObject(dt.name);
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
