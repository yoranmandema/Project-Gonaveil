using UnityEngine;

public class NetworkPlayerController : MonoBehaviour
{
    public int connectionID;

    public Connection networkController;
    private Rigidbody rb;

    void Start()
    {
        networkController = GetComponentInParent<Connection>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
