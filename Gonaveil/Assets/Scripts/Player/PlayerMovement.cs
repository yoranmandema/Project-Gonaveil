using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    public float maxVelocity = 5f;
    public float acceleration = 50f;

    public float airAccelaration = 25f;
    public float airDrag = 0.25f;

    public bool autoJump;
    public float jumpHeight = 1f;
    public float jumpLateralSpeedMultiplier = 1.1f;
    public float fallMultiplierFloat = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public bool isOnWalkableGround;
    public bool isGrounded;
    public float maxGroundInclination = 0.75f;

    public LayerMask groundedLayerMask;

    public Vector3 groundedNormal;
    public Vector3 lastGroundedNormal;

    private CharacterController characterController;
    private Vector3 velocity;
    private Rigidbody rb;
    private float lateralJumpVelocity;

    private bool wantsJump;
    private bool canJump = true;

    void Start() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    private bool WantsJumpInput () {
        if (autoJump) {
            return Input.GetButton("Jump");
        } else {
            return Input.GetButtonDown("Jump");
        }
    }

    void Update() {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.up * characterController.radius, characterController.radius + 0.1f, groundedLayerMask, QueryTriggerInteraction.Ignore);

        Debug.DrawLine(transform.position + Vector3.up * characterController.radius, transform.position + Vector3.up * characterController.radius - Vector3.up * (characterController.radius + 0.01f));

        var desiredVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        var transformedVelocity = transform.TransformDirection(desiredVelocity);

        if (isGrounded) {
            if (canJump) canJump = false;

            desiredVelocity.Normalize();

            velocity += Vector3.ClampMagnitude(transformedVelocity * maxVelocity - velocity, acceleration * Time.deltaTime);

            if (WantsJumpInput() && canJump) {
                velocity = Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude) + Vector3.Scale(velocity, new Vector3(jumpLateralSpeedMultiplier, 0, jumpLateralSpeedMultiplier));

                lateralJumpVelocity = Vector3.Scale(velocity, new Vector3(1,0,1)).magnitude;
            }
        }
        else {
            if (Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude < lateralJumpVelocity) {
                velocity += transformedVelocity * airAccelaration * Time.deltaTime;
            }

            velocity -= velocity * airDrag * Time.deltaTime;

            if (velocity.y < 0 && velocity.y > -fallMaxSpeedUp) {
                velocity += Vector3.up * Physics.gravity.y * (fallMultiplierFloat - 1) * Time.deltaTime;
            }

            velocity += Physics.gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
