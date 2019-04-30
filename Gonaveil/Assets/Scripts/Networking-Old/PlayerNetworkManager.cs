using UnityEngine;
using Networking;

public class PlayerNetworkManager : MonoBehaviour
{
    Connection connection;
    CharacterController charController;

    void Start()
    {
        connection = GameObject.Find("NetworkingController").GetComponent<Connection>();
        charController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if(connection.IsRunning())
        {
            UpdatePlayerPositionAndState message = new UpdatePlayerPositionAndState((byte)NetMessageType.UpdatePlayerPostionAndState);
            message.PlayerID = (byte)connection.ConnectionID();
            message.Sliding = false;
            message.Grounded = true;
            message.Pos[0] = gameObject.transform.position.x;
            message.Pos[1] = gameObject.transform.position.y;
            message.Pos[2] = gameObject.transform.position.z;
            message.Rot[0] = gameObject.transform.rotation.w;
            message.Rot[1] = gameObject.transform.rotation.x;
            message.Rot[2] = gameObject.transform.rotation.y;
            message.Rot[3] = gameObject.transform.rotation.z;
            message.Vel[0] = charController.velocity.x;
            message.Vel[1] = charController.velocity.x;
            message.Vel[2] = charController.velocity.x;

            if (connection.isHost)
            {
                connection.Broadcast(connection.UnreliableChannelID(), message);
            }
            else
            {
                connection.Send(connection.ConnectionID(), connection.UnreliableChannelID(), message);
            }
        }
    }
}
