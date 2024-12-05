using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
    void close()
    {
        gameObject.SetActive(false);
    }
    public void ChangeSim()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UI");
    }
    public void ChangeMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StarData");
    }
}
