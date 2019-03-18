using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxVelocity = 5f;
    public float acceleration = 50f;
    public float decceleration = 50f;

    public bool isGrounded;

    private Vector3 velocity;
    private Rigidbody rb;
    private Collider collider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    void Update () {
        var desiredVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * maxVelocity;
        var transformedVelocity = transform.TransformDirection(desiredVelocity);

        velocity += Vector3.ClampMagnitude(transformedVelocity - velocity, acceleration * Time.deltaTime);
    }

    void FixedUpdate()
    {    
        isGrounded = Physics.CheckCapsule(collider.bounds.center, new Vector3(collider.bounds.center.x, collider.bounds.min.y - 0.1f, collider.bounds.center.z), 0.18f);

        var velocityY = rb.velocity.y;

        rb.velocity = velocity.SetY(velocityY);
    }
}
