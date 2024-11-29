using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WebSocketManager : MonoBehaviour
{
    private WebSocket ws;
    public event Action<string> OnReceive;

    void Start()
    {
        // WebSocket 초기화
        ws = new WebSocket("wss://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/ws");
        //ws = new WebSocket("ws://localhost:8000/ws");

        // 서버에서 데이터를 받을 때 호출되는 이벤트 핸들러
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received Data: " + e.Data);

            OnReceive?.Invoke(e.Data);
        };

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("SUCCESS");
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log(e.Message);
        };

        // WebSocket 서버에 연결
        try
        {
            ws.Connect();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        

        // 초기 메시지를 서버에 전송
        //SendDataToServer();
    }

    // 서버에 데이터를 보낼 때 사용
    void SendDataToServer()
    {
        if (ws != null && ws.IsAlive)
        {
            // 예시 데이터
            string data = "{\"location\":{\"lat\":37.5665, \"lon\":126.9780}}";
            Debug.Log("Sending data to server: " + data);
            ws.Send(data);
        }
    }

    // WebSocket 연결 해제
    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
