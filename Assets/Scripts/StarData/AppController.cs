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
    private DistanceManager distanceManager;
    void Start()
    {
        webSocketManager = FindObjectOfType<WebSocketManager>();
        dataManager = FindObjectOfType<DataManager>();
        objectManager = FindObjectOfType<ObjectManager>();
        distanceManager = FindObjectOfType<DistanceManager>();

        webSocketManager.OnReceive += HandleMessageReceived;
    }

    private void HandleMessageReceived(string json)
    {
        dataManager.ParseData(json);
        objectManager.UpdateObjects();
    }
}
