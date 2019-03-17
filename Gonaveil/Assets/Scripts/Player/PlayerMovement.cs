using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxVelocity = 5f;
    public float acceleration = 50f;
    public float decceleration = 50f;

    private Vector3 velocity;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var velocityY = rb.velocity.y;
        var localVel = transform.InverseTransformDirection(rb.velocity);

        var forwardAcceleration = -localVel.z * decceleration;
        var sideAcceleration = -localVel.x * decceleration;

        if (Input.GetAxis("Vertical") != 0) forwardAcceleration = Input.GetAxis("Vertical") * acceleration;
        if (Input.GetAxis("Horizontal") != 0) sideAcceleration = Input.GetAxis("Horizontal") * acceleration;

        rb.velocity = 
            rb.velocity + 
            (transform.forward * forwardAcceleration + 
            transform.right * sideAcceleration) * Time.deltaTime;



        rb.velocity = Vector3.ClampMagnitude(Vector3.Scale(rb.velocity,new Vector3(1, 0, 1)), maxVelocity).SetY(velocityY);
    }
}
