using UnityEngine;
using Networking;

public class PlayerNetworkManager : MonoBehaviour
{
    private Connection connection;
    private Rigidbody rb;

    void Start()
    {
        connection = GameObject.Find("NetworkingController").GetComponent<Connection>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(connection.IsRunning())
        {
            UpdatePlayerPositionAndState message = new UpdatePlayerPositionAndState((byte)NetMessageType.UpdatePlayerPostionAndState);
            message.PlayerID = (byte)connection.ConnectionID();
            message.Sliding = false;
            message.Grounded = true;
            message.Pos[0] = transform.position.x;
            message.Pos[1] = transform.position.y;
            message.Pos[2] = transform.position.z;
            message.Rot[0] = transform.rotation.w;
            message.Rot[1] = transform.rotation.x;
            message.Rot[2] = transform.rotation.y;
            message.Rot[3] = transform.rotation.z;
            message.Vel[0] = rb.velocity.x;
            message.Vel[1] = rb.velocity.y;
            message.Vel[2] = rb.velocity.z;

            if (connection.isHost)
            {
                connection.Broadcast(connection.ReliableChannelID(), message);
            }
            else
            {
                connection.SendServer(connection.ReliableChannelID(), message);
            }
        }
    }
}
