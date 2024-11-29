using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollView : MonoBehaviour
{
    [SerializeField]
    private GameObject item;
    [SerializeField]
    private GameObject content;
    private JSON_Parser parser;

    public void MakeItem(string json)
    {
        GameObject newItem = GameObject.Instantiate(item, content.transform);
        parser = new JSON_Parser();
        Star_data tmp = parser.readJSON(json);
        newItem.GetComponent<ItemBehavior>().setData(tmp);
        newItem.GetComponent<ItemBehavior>().SetName(tmp.nameUnicode);
        newItem.GetComponent<ItemBehavior>().SImg(tmp.img);
    }
}