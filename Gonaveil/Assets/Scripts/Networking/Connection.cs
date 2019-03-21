using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Networking;

#pragma warning disable CS0618 // Type or member is obsolete
public class Connection : MonoBehaviour
{
    //Constants
    private readonly int byteSize = 256;

    public bool autoInit;

    //Server related
    public bool isHost = true;
    private readonly int maxConnections = 32;
    private readonly int socketPort = 22222;

    //Client related
    public string serverAddress = "127.0.0.1";
    private int connectionID;

    //Prefab configuration
    public GameObject networkPlayerPrefab;

    //State variables
    private bool isHosting;
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

        if (isHost)
        {
            hostID = NetworkTransport.AddHost(topology, socketPort, null);
            Debug.Log(string.Format("Hosting server on port {0}", socketPort));

            isRunning = true;
            isHosting = true;
        }
        else
        {
            hostID = NetworkTransport.AddHost(topology, 0);
            connectionID = NetworkTransport.Connect(hostID, serverAddress, socketPort, 0, out error);
            Debug.Log(string.Format("Connecting to {0} on port {1}", serverAddress, socketPort));

            isRunning = true;
            isHosting = false;
        }
    }

    void Update()
    {
        UpdateNetworkMessage();
    }

    void UpdateNetworkMessage()
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
                Debug.Log(string.Format("Received data from client {0}", clientConnectionID));

                BinaryFormatter formater = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream(buffer);
                Message message = (Message)formater.Deserialize(memoryStream);

                HandleMessage(connectionID, channelID, hostID, message);
                break;
        }
    }

    public void Shutdown()
    {
        isRunning = false;
        isHosting = false;
        NetworkTransport.Shutdown();
        Debug.Log("Network stopped");
    }

    public void SendServer(Message message)
    {
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formater = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(buffer);
        formater.Serialize(memoryStream, message);

        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, buffer.Length, out error);
    }

    public void SendClient(Message message)
    {
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formater = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(buffer);
        formater.Serialize(memoryStream, message);

        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, buffer.Length, out error);
    }

    #region HandleMessage
    void HandleMessage(int receivingConnectionID, int receivingChannelID, int receivingHostID, Message message)
    {
        switch(message.MessageType)
        {
            case (byte)NetMessageType.ConnectionInfo:
                ConnectionInfo info = (ConnectionInfo)message;
                Debug.Log(info.Msg);
                break;
            case (byte)NetMessageType.UpdatePlayerPostionAndState:

                break;
            case (byte)NetMessageType.UpdatePlayerData:

                break;
            case (byte)NetMessageType.UpdatePropPostionAndState:

                break;
            case (byte)NetMessageType.BulletHit:

                break;
            case (byte)NetMessageType.WeaponUsed:

                break;
            case (byte)NetMessageType.WeaponPickUp:

                break;
            case (byte)NetMessageType.WeaponDrop:

                break;
            case (byte)NetMessageType.ChatMessage:

                break;
            case (byte)NetMessageType.LevelInfo:

                break;
            case (byte)NetMessageType.LevelChange:

                break;
        }
    }
    #endregion
}
#pragma warning restore CS0618 // Type or member is obsolete