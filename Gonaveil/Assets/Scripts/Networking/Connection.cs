using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Networking;

#pragma warning disable CS0618 // Type or member is obsolete
public class Connection : MonoBehaviour
{
    //Constants
    private readonly int byteSize = 1024;

    public bool autoInit;

    //Server related
    public bool isHost = true;
    private readonly int maxConnections = 32;
    private readonly int socketPort = 22222;

    //Client related
    public string serverAddress = "localhost";
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

    private bool[] userConnected;
    private GameObject[] players;

    public byte ReliableChannelID()
    {
        return reliableChannelID;
    }

    public byte UnreliableChannelID()
    {
        return unreliableChannelID;
    }

    public int ConnectionID()
    {
        return connectionID;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        userConnected = new bool[maxConnections];
        players = new GameObject[maxConnections];
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
                userConnected[clientConnectionID] = true;
                AddPlayer(clientConnectionID);
                break;

            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("Client disconnected. ID{0}", clientConnectionID));
                userConnected[clientConnectionID] = false;
                RemovePlayer(clientConnectionID);
                break;

            case NetworkEventType.DataEvent:
                //Debug.Log(string.Format("Received data from client {0}", clientConnectionID));

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

        NetworkTransport.Disconnect(hostID, connectionID, out error);
        NetworkTransport.Shutdown();
        Debug.Log("Network stopped");
    }

    public void Send(int userID, int channelID, Message message)
    {
        byte[] buffer = new byte[byteSize];

        BinaryFormatter formater = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(buffer);
        formater.Serialize(memoryStream, message);

        NetworkTransport.Send(hostID, userID, reliableChannelID, buffer, buffer.Length, out error);
    }

    void RelayMessage(int receivingConnectionID, int receivingChannelID, int receivingHostID, Message message)
    {
        for (int i = 0; i < maxConnections; i++)
        {
            if (i != receivingConnectionID && userConnected[i])
            {
                Send(i, receivingChannelID, message);
            }
        }
    }

    public void Broadcast(int channel, Message message)
    {
        for (int i = 0; i < maxConnections; i++)
        {
            if (userConnected[i])
            {
                Send(i, channel, message);
            }
        }
    }

    void AddPlayer(int userID)
    {
        players[userID] = Instantiate(networkPlayerPrefab);
        players[userID].transform.SetParent(gameObject.transform);
    }

    void RemovePlayer(int userID)
    {
        Destroy(players[userID].gameObject);
    }

    void UpdatePlayerPositionAndState(int clientID, UpdatePlayerPositionAndState data)
    {
        Vector3 pos = new Vector3(data.Pos[0], data.Pos[1], data.Pos[2]);
        Quaternion rot = new Quaternion(data.Rot[0], data.Rot[1], data.Rot[2], data.Rot[3]);
        Vector3 vel = new Vector3(data.Vel[0], data.Vel[1], data.Vel[2]);
        //CharacterController charController = players[clientID].GetComponent<CharacterController>();
        Debug.Log("X: " + data.Pos[0] + " Y: " + data.Pos[1] + " Z: " + data.Pos[2]);
        players[clientID].transform.SetPositionAndRotation(pos, rot);
        players[clientID].GetComponent<Rigidbody>().velocity = vel;
    }

    #region HandleMessage
    void HandleMessage(int receivingConnectionID, int receivingChannelID, int receivingHostID, Message message)
    {
        switch (message.MessageType)
        {
            case (byte)NetMessageType.ConnectionInfo:
                ConnectionInfo info = (ConnectionInfo)message;
                Debug.Log(info.Msg);
                break;
            case (byte)NetMessageType.UpdatePlayerPostionAndState:
                //Debug.Log("New position data");
                UpdatePlayerPositionAndState(receivingConnectionID, (UpdatePlayerPositionAndState)message);
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