using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

enum MessageType
{
    UpdatePlayerPostionAndState,
    UpdatePlayerData,
    UpdatePropPostionAndState,
    BulletHit,
    WeaponUsed,
    WeaponPickUp,
    WeaponDrop,
    ChatMessage,
    LevelInfo,
    LevelChange,
}
#pragma warning disable CS0618 // Type or member is obsolete
public class Connection : MonoBehaviour
{
    //Constants
    private readonly int byteSize = 256;

    public bool autoInit;

    //Server related
    public bool isHosting = true;
    private readonly int maxConnections = 32;
    private readonly int socketPort = 22222;

    //Client related
    public string serverAddress = "127.0.0.1";
    private int connectionID;

    //Prefab configuration
    public GameObject networkPlayerPrefab;

    //State variables
    private byte unreliableChannelID;
    private byte reliableChannelID;
    private int hostID;
    private byte error;
    private bool isRunning;

    public bool IsRunning()
    {
        return isRunning;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (autoInit) Init();
    }

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);

        HostTopology topology = new HostTopology(config, maxConnections);

        if (isHosting)
        {
            hostID = NetworkTransport.AddHost(topology, socketPort, null);
            Debug.Log(string.Format("Hosting server on port {0}", socketPort));

            isRunning = true;
        }
        else
        {
            hostID = NetworkTransport.AddHost(topology, 0);
            connectionID = NetworkTransport.Connect(hostID, serverAddress, socketPort, 0, out error);
            Debug.Log(string.Format("Connecting to {0} on port {1}", serverAddress, socketPort));

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


        byte[] buffer = new byte[byteSize];

        NetworkEventType eventType = NetworkTransport.Receive(out int receivingHostID, out int clientConnectionID, out int channelID, buffer, buffer.Length, out int dataSize, out error);

        switch (eventType)
        {
            case NetworkEventType.Nothing:
                break;

            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("Client connected. ID {0}", clientConnectionID));
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("Client disconnected. ID{0}", clientConnectionID));
                break;

            case NetworkEventType.DataEvent:
                Debug.Log(string.Format("Received data from client {0}. Data: {1}", clientConnectionID, buffer));
                break;
        }
    }

    public void Shutdown()
    {
        isRunning = false;
        NetworkTransport.Shutdown();
        Debug.Log("Network stopped");
    }

    //Relevant
    public void SendServer()
    {

    }

    public void SendClient()
    {

    }
}
#pragma warning restore CS0618 // Type or member is obsolete