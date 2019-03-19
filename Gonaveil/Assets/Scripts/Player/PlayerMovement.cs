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

    private void CheckIfGrounded() {
        wasGrounded = isGrounded;

        //isGrounded = Physics.CheckSphere(transform.position + Vector3.up * 0.25f, 0.35f, groundedLayerMask, QueryTriggerInteraction.Ignore);

        isGrounded = characterController.isGrounded;
    }

    Vector3 GetStrafeAcceleration() {
        float forward = Input.GetAxis("Vertical");
        float left = Input.GetAxis("Horizontal");
        float yaw = transform.eulerAngles.y * Mathf.Deg2Rad;

        Vector3 normalizedVelocity = velocity.normalized;

        Vector3 rightOfVelocity = new Vector3(velocity.z, 0, -velocity.x);

        Vector3 control = new Vector3(forward * Mathf.Sin(yaw) + left * Mathf.Cos(yaw), 0, forward * Mathf.Cos(yaw) + left * Mathf.Sin(yaw));

        return rightOfVelocity * Vector3.Dot(rightOfVelocity, control);
    }

    void Update() {
        CheckIfGrounded();

        characterController.Move(velocity * Time.deltaTime);

        desiredMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (isGrounded) {
            if (!wasGrounded) canJump = true;

            GroundMovement();
        }
        else {
            AirMovement();
        }
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

        // Lateral air acceleration.
        if (Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude < lateralJumpVelocity) {
            var transformedMovement = transform.TransformDirection(desiredMovement);

            velocity += transformedMovement * airAccelaration * Time.deltaTime;
        }

        //velocity += GetStrafeAcceleration();

        // Air drag.
        velocity -= velocity * airDrag * Time.deltaTime;

        // Faster fall velocity.
        if (velocity.y < 0 && velocity.y > -fallMaxSpeedUp) {
            velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        }

        // Gravity.
        velocity += Physics.gravity * Time.deltaTime;
    }
}
