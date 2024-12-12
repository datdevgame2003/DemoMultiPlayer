using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class UINetworkManager : MonoBehaviour
{
    private string ipAddress = "127.0.0.1"; //dia chi ip
    private ushort port = 7777;//cong ket noi

    void Awake()
    {
        DontDestroyOnLoad(gameObject);//doi tuong khong bi huy khi chuyen doi giua cac scene
    }
    void OnGUI() //giao dien 
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));//gioi han ui
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }
        GUILayout.EndArea();//gioi han ui
    }

    void StartButtons()
    {
        GUILayout.Label("IP Address:");
        ipAddress = GUILayout.TextField(ipAddress); //nhap ip

        GUILayout.Label("Port:");
        port = ushort.Parse(GUILayout.TextField(port.ToString())); //nhap port

        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost(); //chay host
        }
        if (GUILayout.Button("Join"))
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>(); //chay client
            transport.SetConnectionData(ipAddress, port);
            NetworkManager.Singleton.StartClient();
        }
        if (GUILayout.Button("Server"))
        {

            NetworkManager.Singleton.StartServer(); //chay server
        }
    }

    void StatusLabels()
    { //kiem tra host client server
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);//lay giao thuc mang
        GUILayout.Label("Mode: " + mode);
    }
   
}
