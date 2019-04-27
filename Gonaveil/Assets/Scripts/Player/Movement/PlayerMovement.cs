using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public partial class PlayerMovement : MonoBehaviour {
    public bool allowInput = true;

    public Transform cameraTransform;
    public float cameraHeight = 1.8f;

    public float maxVelocity = 12f;
    public float acceleration = 250f;
    public float friction = 15f;
    public float frictionTime = 0.1f;
    public float stepSlope = 85f;

    public float airAcceleration = 150f;
    public float airDrag = 0.0f;
    public float maxAirVelocity = 1f;
    public float fallSpeedMultiplier = 3f;
    public float fallMaxSpeedUp = 10f;

    public float jumpHeight = 1f;
    public float jumpLateralSpeedMultiplier = 1.1f;
    public bool autoJumping;
    public bool queueJumping;
    public float jumpCooldown = 0.25f;
    public float upHillJumpBoost = 5f;

    public float surfSlope = 45f;
    public float surfAcceleration = 150f;

    public float crouchHeight = 1f;
    public float crouchTime = 0.1f;
    public float crouchVelocity = 6f;
    public float crouchCameraHeight = 0.9f;

    public float slideBoost = 1.25f;
    public float slideVelocityThreshold = 10f;

    public float flipTime = 0.25f;

    public bool isGrounded;
    public bool isInAir;
    public bool isSurfing;
    public bool isCrouching;
    public bool isSliding;

    public Vector3 velocity;

    private CharacterController characterController;
    private Vector3 groundNormal;
    private Vector3 groundPoint;
    private Vector3 desiredMovement;
    private bool wasGrounded;
    private float appliedCrouchHeight = 2f;
    private float crouchLerp = 1f;
    private float desiredCrouchLerp = 1f;
    private bool isOnSlope;

    private bool canJump = true;
    private bool canJumpCooldown = true;
    private bool queuedJump;

    private bool canApplyFriction;
    private float frictionMul;

    private WeaponMovement weaponMovement;

    private Vector3 TransformedMovement => transform.TransformDirection(desiredMovement);
    private Vector3 ProjectedMovement => Vector3.ProjectOnPlane(TransformedMovement, groundNormal).normalized;
    private float GroundSlope => Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up)) * Mathf.Rad2Deg;

    private void Start() {
        characterController = GetComponent<CharacterController>();
        weaponMovement = GetComponentInChildren<WeaponMovement>();
    }

    private bool WantsJumpInput() {
        if (autoJumping) {
            return Input.GetButton("Jump");
        }
        else {
            return Input.GetButtonDown("Jump");
        }
    }

    private IEnumerator JumpCooldown() {
        canJumpCooldown = false;

        yield return new WaitForSeconds(jumpCooldown);

        canJumpCooldown = true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        groundNormal = hit.normal;
    }

    void Update() {
        OnScreenDebug.Print($"Vel: {velocity.SetY(0).magnitude}");
        OnScreenDebug.Print($"{characterController.isGrounded}");

        if (allowInput) {
            if (Input.GetButtonDown("Crouch") && velocity.magnitude > slideVelocityThreshold) {
                isSliding = true;

                if (isGrounded) {
                    velocity *= slideBoost;
                }
            }
            else if (Input.GetButtonUp("Crouch") && isSliding) {
                isSliding = false;
            }
        }

        velocity.y -= 0.1f;
        characterController.Move(velocity * Time.deltaTime);

        // Prevent the input velocity from getting bigger than what the real velocity is.
        // This prevents the player from shooting off in a certain direction when losing contact.
        if (velocity.magnitude > characterController.velocity.magnitude) {
            velocity = characterController.velocity;
        }

        desiredMovement = allowInput ? new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized : Vector3.zero;

        isOnSlope = characterController.isGrounded && (GroundSlope > surfSlope);

        isGrounded = characterController.isGrounded && !isOnSlope;

        if (isGrounded) {
            if (isSliding) {
                SlideMovement();
            }
            else {
                GroundMovement();
            }
        }
        else {
            AirMovement();
        }

        CrouchMovement();

        if (wasGrounded != isGrounded && isGrounded) {
            OnEnterGrounded();
        }
        else if (wasGrounded != isGrounded && !isGrounded) {
            OnEnterAir();
        }

        wasGrounded = characterController.isGrounded;
    }

    private void SlideMovement() {
        var upwards = Vector3.Dot(groundNormal, Vector3.up);
        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;

        JumpMovement();
    }

    private void CrouchMovement() {
        isCrouching = Input.GetButton("Crouch") && allowInput;

        desiredCrouchLerp = isCrouching ? 0 : 1f;

        crouchLerp = Mathf.Max(Mathf.Min(crouchLerp + (desiredCrouchLerp * 2 - 1) * Time.deltaTime / crouchTime, 1f), 0);

        appliedCrouchHeight = 2f * crouchLerp + crouchHeight * (1 - crouchLerp);

        characterController.height = appliedCrouchHeight;
        characterController.center = Vector3.up * (appliedCrouchHeight / 2f);

        cameraTransform.localPosition = Vector3.up * (cameraHeight * crouchLerp + crouchCameraHeight * (1 - crouchLerp));

        // Sort of enables crouch jumping.
        if (isInAir) {
            characterController.Move(Vector3.up * -(desiredCrouchLerp - crouchLerp) * Time.deltaTime / crouchTime * 2f);
        }
    }

    private void JumpMovement() {
        if ((WantsJumpInput() || queuedJump) && canJumpCooldown) {
            canJump = false;
            queuedJump = false;

            weaponMovement.Impulse(
               new Vector3(0, -2, 0),
               new Vector2(-50, 0)
               );

            StartCoroutine(JumpCooldown());

            var lateralVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)) * jumpLateralSpeedMultiplier;
            var jumpVelocity = Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude * (fallSpeedMultiplier - 1));

            // Boost the jump when going uphill.
            if (ProjectedMovement.y > 0) {
                jumpVelocity += Vector3.up * lateralVelocity.magnitude * upHillJumpBoost * (1 - Vector3.Dot(groundNormal, Vector3.up));
            }

            velocity = jumpVelocity + lateralVelocity;
        }
    }

    private void OnEnterGrounded() {
        StartCoroutine(DisableFrictionForFrame());
    }
    private void OnEnterAir() {
        frictionMul = 0f;
    }

    private void GroundMovement() {
        frictionMul = Mathf.Min(frictionMul + Time.deltaTime / frictionTime, 1);

        var maxVel = isCrouching && !isSliding ? crouchVelocity : maxVelocity;

        var speed = velocity.magnitude;

        if (speed != 0 && canApplyFriction) {
            var drop = speed * friction * frictionMul * Time.deltaTime;
            velocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction.
        }

        DoAcceleration(transform.TransformDirection(desiredMovement), acceleration, maxVel);

        JumpMovement();
    }

    private IEnumerator DisableFrictionForFrame() {
        canApplyFriction = false;

        yield return new WaitForSeconds(Time.deltaTime * 3f);

        canApplyFriction = true;
    }

    private void AirMovement() {
        if (queueJumping && Input.GetButtonDown("Jump")) queuedJump = true;

        DoAcceleration(transform.TransformDirection(desiredMovement), airAcceleration, maxAirVelocity);

        var upwards = Vector3.Dot(groundNormal, Vector3.up);
        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.

        if (GroundSlope < surfSlope) {
            downVector = Vector3.down;
        }

        // Air drag / friction.
        velocity -= velocity * airDrag * Time.deltaTime;

        // Faster fall velocity.
        if (velocity.y > -fallMaxSpeedUp) velocity += downVector * -Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;
    }

    private void DoAcceleration(Vector3 wishDirection, float maxAccel, float maxVel) {
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
        velocity += force * Time.deltaTime;
    }
}
