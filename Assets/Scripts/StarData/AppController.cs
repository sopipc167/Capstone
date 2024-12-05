using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using WebSocketSharp.Server;

public class StarManager : MonoBehaviour
{
    private WebSocketManager webSocketManager;
    private DataManager dataManager;
    private ObjectManager objectManager;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        webSocketManager = FindObjectOfType<WebSocketManager>();
        dataManager = FindObjectOfType<DataManager>();
        objectManager = FindObjectOfType<ObjectManager>();

        webSocketManager.OnReceive += HandleMessageReceived;
    }

    private void HandleMessageReceived(string json)
    {
        if (!dataManager.isUpdatedSunPositions)
        {
            dataManager.ParseSunData(json);
        }
        else
        {
            dataManager.ParseData(json);
        }
        if (dataManager.isUpdated)
            objectManager.UpdateObjects();
    }
}
