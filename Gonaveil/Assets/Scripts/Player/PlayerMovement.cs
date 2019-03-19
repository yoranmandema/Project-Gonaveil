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
    public float fallSpeedMultiplier = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public bool isOnWalkableGround;
    public bool isGrounded;
    public float maxGroundInclination = 0.75f;

    public LayerMask groundedLayerMask;

    private CharacterController characterController;
    private Vector3 velocity;
    private Vector3 desiredMovement;
    private Rigidbody rb;
    private float lateralJumpVelocity;

    private bool wantsJump;
    private bool canJump = true;
    private bool wasGrounded;

    private void Start() {
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

    private void CheckIfGrounded () {
        wasGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(transform.position + Vector3.up * (characterController.radius - 0.2f), characterController.radius - 0.1f, groundedLayerMask, QueryTriggerInteraction.Ignore);
    }

    void Update() {
        CheckIfGrounded();

        desiredMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (isGrounded) {
            if (!wasGrounded) canJump = true;

            GroundMovement();
        }
        else {
            AirMovement();
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void GroundMovement () {
        desiredMovement.Normalize();

        var transformedMovement = transform.TransformDirection(desiredMovement);

        velocity += Vector3.ClampMagnitude(transformedMovement * maxVelocity - velocity, acceleration * Time.deltaTime);

        if (WantsJumpInput() && canJump) {
            canJump = false;

            velocity = Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude) + Vector3.Scale(velocity, new Vector3(jumpLateralSpeedMultiplier, 0, jumpLateralSpeedMultiplier));

            lateralJumpVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude;
        }
    }

    private void AirMovement () {

        // Faster fall velocity.
        if (Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude < lateralJumpVelocity) {
            var transformedMovement = transform.TransformDirection(desiredMovement);

            velocity += transformedMovement * airAccelaration * Time.deltaTime;
        }

        // Air drag.
        velocity -= velocity * airDrag * Time.deltaTime;

        // Lateral air acceleration.
        if (velocity.y < 0 && velocity.y > -fallMaxSpeedUp) {
            velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        }

        // Gravity.
        velocity += Physics.gravity * Time.deltaTime;
    }
}
