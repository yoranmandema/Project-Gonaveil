using UnityEngine;
using System.Collections;
using Networking;

public class NetworkPlayerController : MonoBehaviour
{
    public int connectionID;

    public Connection networkController;
    private Transform tf;
    private Rigidbody rb;

    void Start()
    {
        networkController = GetComponentInParent<Connection>();
        tf = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
