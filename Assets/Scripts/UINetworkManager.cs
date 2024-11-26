using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class UINetworkManager : MonoBehaviour
{
    private string ipAddress = "127.0.0.1";
    private ushort port = 7777;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else if (!NetworkManager.Singleton.IsListening)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }
        GUILayout.EndArea();
    }

    void StartButtons()
    {
        GUILayout.Label("IP Address:");
        ipAddress = GUILayout.TextField(ipAddress);

        GUILayout.Label("Port:");
        port = ushort.Parse(GUILayout.TextField(port.ToString()));

        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();
        }
        if (GUILayout.Button("Join"))
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ipAddress, port);
            NetworkManager.Singleton.StartClient();
        }
        if (GUILayout.Button("Server"))
        {

            NetworkManager.Singleton.StartServer();
        }
    }

    void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
   
}
