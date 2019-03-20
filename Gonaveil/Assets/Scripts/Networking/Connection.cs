using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Connection : MonoBehaviour
{
    public bool hostServer = true;
    private readonly int maxConnections = 32;
    private readonly int socketPort = 22222;

    private readonly int byteSize = 256;

    public bool connectToServer = false;
    public string serverAddress = "127.0.0.1";

    public GameObject networkPlayerPrefab;

    private byte unreliableChannelID;
    private byte reliableChannelID;
    private int hostID;
    private byte error;

    private bool isRunning = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);

        HostTopology topology = new HostTopology(config, maxConnections);

        if (hostServer)
        {
            hostID = NetworkTransport.AddHost(topology, socketPort, null);
            Debug.Log("Hosting server on port " + socketPort);

            isRunning = true;
        }
        else if (connectToServer)
        {
            hostID = NetworkTransport.AddHost(topology, 0);
            NetworkTransport.Connect(hostID, serverAddress, socketPort, 0, out error);
            Debug.Log("Connecting to " + serverAddress + " on port " + socketPort);

            isRunning = true;
        }
    }

    void Update()
    {
        UpdateNetworkMessage();
    }

    public void UpdateNetworkMessage()
    {
        if (!isRunning) return;

        int receivingHostID;
        int connectionID;
        int channelID;

        byte[] buffer = new byte[byteSize];
        int dataSize;

        NetworkEventType eventType = NetworkTransport.Receive(out receivingHostID, out connectionID, out channelID, buffer, buffer.Length, out dataSize, out error);

        switch(eventType)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log("Client connected. ID " + connectionID);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log("Client disconnected. ID" + connectionID);
                break;

            case NetworkEventType.DataEvent:
                Debug.Log("Received data from client " + connectionID + ". Data: " + buffer);
                break;
        }
    }

    public void Shutdown()
    {
        isRunning = false;
        NetworkTransport.Shutdown();
    }
}
