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
        // WebSocket �ʱ�ȭ (Python ������ ����)
        ws = new WebSocketSharp.WebSocket("wws://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/ws");

        // �������� �����͸� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received Data: " + e.Data);

            // �������� ���� �����͸� ó���ϴ� ��
        };

        // WebSocket ������ ����
        ws.Connect();

        // �ʱ� �޽����� ������ ����
        SendDataToServer();
    }

    // ������ �����͸� ���� �� ���
    void SendDataToServer()
    {
        if (ws != null && ws.IsAlive)
        {
            // ���� ������
            string data = "{\"location\":[37.5665, 126.9780]}";
            Debug.Log("Sending data to server: " + data);
            ws.Send(data);
        }
    }

    // WebSocket ���� ����
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