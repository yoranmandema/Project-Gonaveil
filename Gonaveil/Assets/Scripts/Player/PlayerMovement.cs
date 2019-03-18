using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float maxVelocity = 5f;
    public float acceleration = 50f;
    public float airAccelaration = 25f;
    public float airDrag = 0.25f;
    public float jumpHeight = 1f;

    public bool isOnWalkableGround;
    public bool isGrounded;
    public float maxGroundInclination = 0.75f;

    public LayerMask groundedLayerMask;

    public Vector3 groundedNormal;
    public Vector3 lastGroundedNormal;

    private Vector3 velocity;
    private Rigidbody rb;
    private Collider collider;
    private bool wasGrounded;

    void Start() {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    void Update() {
        CheckGrounded();

        var desiredVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var transformedVelocity = transform.TransformDirection(desiredVelocity);

        if (isOnWalkableGround) {
            desiredVelocity.Normalize();

            velocity += Vector3.ClampMagnitude(transformedVelocity * maxVelocity - velocity, acceleration * Time.deltaTime);

            if (Input.GetButtonDown("Jump")) rb.velocity = rb.velocity.SetY(Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude));
        }
        else {
            velocity += transformedVelocity * airAccelaration * Time.deltaTime;

            velocity -= velocity * airDrag * Time.deltaTime;

            if (isGrounded) {
                var planeDown = Vector3.ProjectOnPlane(Vector3.down, groundedNormal);

                velocity += planeDown * Time.deltaTime * -Physics.gravity.y * Vector3.Dot(planeDown, Vector3.down);
            }
        }

        if (!isGrounded) velocity = velocity.SetY(0);
    }

    private void CheckGrounded() {
        wasGrounded = isGrounded || isOnWalkableGround;

        isOnWalkableGround = false;
        isGrounded = false;
        groundedNormal = Vector3.zero;

        var rayCasts = Physics.SphereCastAll(collider.bounds.center, 0.25f, Vector3.down, collider.bounds.extents.y, groundedLayerMask);

        foreach (var cast in rayCasts) {
            if (Vector3.Dot(cast.normal, Vector3.up) > maxGroundInclination) {
                isOnWalkableGround = true;

                lastGroundedNormal = groundedNormal;
            }

            if (cast.normal.y > 0) {
                groundedNormal += cast.normal;

                isGrounded = true;
            }
        }

        if (wasGrounded != isGrounded && isGrounded) {
            if (lastGroundedNormal != groundedNormal)
                OnGrounded();

            lastGroundedNormal = groundedNormal;
        }

        groundedNormal = groundedNormal.normalized;
    }

    void OnGrounded () {
        Debug.DrawLine(transform.position, transform.position + velocity, Color.blue, 60f);
        var planeDown = Vector3.ProjectOnPlane(Vector3.down, groundedNormal);

        velocity = Vector3.ProjectOnPlane(velocity, groundedNormal) * Mathf.Pow(Vector3.Dot(groundedNormal, Vector3.up),3);

        Debug.DrawLine(transform.position, transform.position + velocity, Color.red, 60f);
    }

    void FixedUpdate() {
        rb.velocity = velocity + Vector3.up * rb.velocity.y;
    }
}
