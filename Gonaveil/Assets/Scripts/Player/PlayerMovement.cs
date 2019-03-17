using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxVelocity = 5f;
    public float acceleration = 50f;

    private Vector3 velocity;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var velocityY = rb.velocity.y;

        rb.velocity = 
            rb.velocity + 
            transform.forward * Input.GetAxis("Vertical") + 
            transform.right * Input.GetAxis("Horizontal");

        rb.velocity = Vector3.ClampMagnitude(Vector3.Scale(rb.velocity,new Vector3(1, 0, 1)), maxVelocity).SetY(velocityY);
    }
}
