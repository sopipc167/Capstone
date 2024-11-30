using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocketSharp.WebSocket ws;

    void Start()
    {
        // WebSocket 초기화 (Python 서버에 연결)
        ws = new WebSocketSharp.WebSocket("wws://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/ws");

        // 서버에서 데이터를 받을 때 호출되는 이벤트 핸들러
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received Data: " + e.Data);

            // 서버에서 받은 데이터를 처리하는 곳
        };

        // WebSocket 서버에 연결
        ws.Connect();

        // 초기 메시지를 서버에 전송
        SendDataToServer();
    }

    // 서버에 데이터를 보낼 때 사용
    void SendDataToServer()
    {
        if (ws != null && ws.IsAlive)
        {
            // 예시 데이터
            string data = "{\"location\":[37.5665, 126.9780]}";
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

    // Update is called once per frame
    void Update()
    {

    }
}