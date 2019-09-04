using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CapsuleCollider))]
public partial class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    public float height = 1.8f;
    public float cameraHeight = 1.8f;
    public float crouchCameraHeight = 0.9f;
    public float crouchHeight = 1f;
    public float crouchTime = 0.5f;
    public float crouchVelocity = 6;

    public float groundDistanceThreshold = 0.01f;

    public float maxVelocity = 12;
    public float acceleration = 100;
    public float friction = 8;

    public float slopeAngle = 45f;

    public float fallMaxSpeedUp = 10f;
    public float airDrag = 0.1f;
    public float airAcceleration = 25f;
    public float jumpHeight = 2f;
    public float fallSpeedMultiplier = 2f;

    public Vector3 velocity;
    public bool wishJump;

    public MovementState movementState;
    public Rigidbody rigidbody;
    public bool isCrouching;
    public bool grounded;
    public Vector3 groundedNormal = Vector3.up;

    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;
    private float crouchLerp;
    private Vector3 desiredMovement;

    void Awake() {
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void OnEnable() {
        SetState(new GroundedState(this));

        print(CommandTerminal.Terminal.Shell);

        CommandTerminal.Terminal.Shell.AddCommand("noclip", EnterNoclip);
    }

    void Update() {
        if (movementState == null) SetState(new GroundedState(this));

        CheckGround();

        desiredMovement = new Vector3(InputManager.GetAxisRaw("Horizontal"), 0, InputManager.GetAxisRaw("Vertical")).normalized;

        if (InputManager.GetButtonDown("Jump") && !wishJump)
            wishJump = true;
        if (InputManager.GetButtonUp("Jump"))
            wishJump = false;

        movementState.OnStateUpdate();

        CrouchMovement();

        characterController.Move(velocity * Time.deltaTime);

        // Prevent the input velocity from getting bigger than what the real velocity is.
        // This prevents the player from shooting off in a certain direction when losing contact.
        if (velocity.magnitude > characterController.velocity.magnitude) velocity = characterController.velocity;
    }

    public void CheckGround() {
        var sweep = rigidbody.SweepTest(Vector3.down, out RaycastHit hit, 1f, QueryTriggerInteraction.Ignore);

        if (sweep) {
            groundedNormal = hit.normal;
        }

        grounded =
            sweep &&
            hit.distance < (characterController.skinWidth + groundDistanceThreshold) &&
            groundedNormal.y > Mathf.Sin(characterController.slopeLimit);
    }

    private void CrouchMovement() {
        isCrouching = InputManager.GetButton("Crouch");

        var desiredCrouchLerp = isCrouching ? 0 : 1f;

        crouchLerp = Mathf.Max(Mathf.Min(crouchLerp + (desiredCrouchLerp * 2 - 1) * Time.deltaTime / crouchTime, 1f), 0);

        float appliedCrouchHeight = height * crouchLerp + crouchHeight * (1 - crouchLerp);

        capsuleCollider.height = characterController.height = appliedCrouchHeight;
        capsuleCollider.center = characterController.center = Vector3.up * (appliedCrouchHeight / height);

        cameraTransform.localPosition = Vector3.up * (cameraHeight * crouchLerp + crouchCameraHeight * (1 - crouchLerp));

        // Sort of enables crouch jumping.
        if (!grounded) {
            characterController.Move(Vector3.up * -(desiredCrouchLerp - crouchLerp) * Time.deltaTime / crouchTime * height);
        }
    }

    public void SetState(MovementState state) {
        movementState?.OnStateExit();

        movementState = state;

        movementState.OnStateEnter();
    }

    public void ApplyFriction(float amount) {
        var vel = velocity;

        if (vel.magnitude != 0) {
            var drop = vel.magnitude * amount * Time.deltaTime;
            velocity *= Mathf.Max(vel.magnitude - drop, 0) / vel.magnitude; // Scale the velocity based on friction.
        }
    }

    public void DoAcceleration(Vector3 wishDirection, float maxAccel, float maxVel) {
        var velocityDelta = GetAcceleration(wishDirection, maxAccel, maxVel);

        velocity += velocityDelta;
    }

    private Vector3 GetAcceleration(Vector3 wishDirection, float maxAccel, float maxVel) {
        var dotVelocity = Vector3.Dot(velocity, wishDirection);
        var addSpeed = maxVel - dotVelocity;
        addSpeed = Mathf.Clamp(addSpeed, 0, maxAccel * Time.deltaTime);

        return wishDirection * addSpeed;
    }

    public void AddForce(Vector3 force) {
        velocity += force / rigidbody.mass;
    }

    public void Move(Vector3 move) {
        characterController.Move(move);
    }

    public void EnterNoclip(CommandTerminal.CommandArg[] args) {
        if (movementState is NoclipState) {
            SetState(new GroundedState(this));
        }
        else {
            SetState(new NoclipState(this));
        }
    }
}
