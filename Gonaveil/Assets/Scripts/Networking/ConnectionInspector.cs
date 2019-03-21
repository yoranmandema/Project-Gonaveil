using UnityEngine;
using System.Collections;
using UnityEditor;
using Networking;

[CustomEditor(typeof(Connection))]
public class ConnectionInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Connection connection = (Connection)target;

        if(connection.IsRunning())
        {
            if(GUILayout.Button("Disconnect/Shutdown")) connection.Shutdown();
            GUILayout.Label("Networking running");
            if(GUILayout.Button("Send Message"))
            {
                connection.SendServer(connection.ReliableChannelID(), new ConnectionInfo((byte)NetMessageType.ConnectionInfo, "Hello, World"));
            }
        }
        else
        {
            if(GUILayout.Button("Connect/Host")) connection.Init();
            GUILayout.Label("Networking stopped");
        }
    }
}
