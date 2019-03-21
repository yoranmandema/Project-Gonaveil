using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    public Transform cameraTransform;
    public float cameraHeight = 1.8f;

    public float maxVelocity = 12f;
    public float acceleration = 200;

    public float airAccelaration = 150;
    public float airDrag = 0.05f;

    public bool limitAirVelocity = false;
    public float fallSpeedMultiplier = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public float jumpHeight = 1f;
    public float jumpLateralSpeedMultiplier = 1.1f;
    public bool autoJump;
    public float jumpCooldown = 0.25f;
    public float upHillJumpBoost = 5f;

    public float surfSlope = 45f;

    public float crouchHeight = 1f;
    public float crouchTime = 0.5f;
    public float crouchVelocity = 8f;
    public float crouchCameraHeight = 0.9f;

    public float slideBoost = 1.25f;
    public float slideVelocityThreshold = 10f;

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
    private Rigidbody rb;
    private LayerMask groundMask;
    private Animator anim;
    private float lateralJumpVelocity;
    private float lateralSurfVelocity;
    private float appliedCrouchHeight = 2f;
    public float crouchLerp = 1f;
    public float desiredCrouchLerp = 1f;

    private bool canJump = true;
    private bool canJumpCooldown = true;
    private bool wasGrounded;
    private bool wasSurfing;
    private bool wasInAir;

    private Vector3 TransformedMovement => transform.TransformDirection(desiredMovement);
    private Vector3 ProjectedMovement => Vector3.ProjectOnPlane(TransformedMovement, groundNormal).normalized;
    private float GroundSlope => Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up)) * Mathf.Rad2Deg;
    private float VelocityDotDirection => Mathf.Max(0, Vector3.Dot(transform.forward, Vector3.Scale(velocity.normalized, new Vector3(1, 0, 1))));

    private void Start() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        groundMask = ((LayerMask)gameObject.layer).GetReverseLayerMask();
        groundMask ^= 1 << gameObject.layer;
    }

    private bool WantsJumpInput() {
        if (autoJump) {
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

    private void GroundCheck() {
        wasSurfing = isSurfing;
        wasGrounded = isGrounded;
        wasInAir = isInAir;

        isGrounded = characterController.isGrounded;
        isSurfing = false;

        var groundHits = Physics.SphereCastAll(
            transform.position + transform.up * characterController.radius,
            characterController.radius,
            -transform.up,
            0.1f,
            groundMask
            );

        var normal = Vector3.zero;
        var hitPoint = Vector3.zero;

        foreach (var hit in groundHits) {
            normal += hit.normal;

            hitPoint += hit.point;

            Debug.DrawLine(hit.point, hit.point + normal, Color.red);
        }

        if (groundHits.Length > 0) {
            normal /= (groundHits.Length);
            normal = normal.normalized;
        }
        else {
            normal = Vector3.up;
        }

        hitPoint /= (groundHits.Length);

        groundNormal = normal;
        groundPoint = hitPoint;

        if (!isGrounded && groundHits.Length > 0) {
            if (GroundSlope <= surfSlope) {
                isGrounded = true;
            }
        }

        if (GroundSlope > surfSlope) {
            isSurfing = true;
            isGrounded = false;
        }

        isInAir = !isGrounded && !isSurfing;

        if (wasSurfing) StartCoroutine(JumpCooldown());
    }

    void Update() {
        GroundCheck();

        if (Input.GetButtonDown("Crouch") && velocity.magnitude > slideVelocityThreshold) {
            isSliding = true;

            if (isGrounded) {
                velocity *= 1;
            }
        }
        else if (Input.GetButtonUp("Crouch") && isSliding) {
            isSliding = false;
        }

        characterController.Move(velocity * Time.deltaTime);

        // Prevent the input velocity from getting bigger than what the real velocity is.
        // This prevents the player from shooting off in a certain direction when losing contact.
        if (velocity.magnitude > characterController.velocity.magnitude) {
            velocity = characterController.velocity;
        }

        desiredMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        if (isSurfing) {
            SurfMovement();
        }
        else if (isSliding && !isInAir) {
            SlideMovement();
        }
        else if (isGrounded) {
            GroundMovement();
        }
        else {
            AirMovement();
        }

        CrouchMovement();

        anim.SetBool("IsCrouching", isCrouching);
        anim.SetFloat("MoveSpeed", velocity.sqrMagnitude);
        if (isSliding) {
            anim.SetTrigger("Slide");
        }
    }

    private void SlideMovement() {
        var upwards = Vector3.Dot(groundNormal, Vector3.up);
        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;

        JumpMovement();
    }

    private void CrouchMovement() {
        isCrouching = Input.GetButton("Crouch");

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
        if (WantsJumpInput() && canJumpCooldown) {
            canJump = false;

            StartCoroutine(JumpCooldown());

            var lateralVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)) * jumpLateralSpeedMultiplier;
            var jumpVelocity =
                Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude * (fallSpeedMultiplier - 1));

            // Boost the jump when going uphill.
            if (ProjectedMovement.y > 0) {
                jumpVelocity += Vector3.up * lateralVelocity.magnitude * upHillJumpBoost * (1 - Vector3.Dot(groundNormal, Vector3.up));
            }

            velocity = jumpVelocity + lateralVelocity;
        }
    }

    private void GroundMovement() {
        if (!wasGrounded) characterController.slopeLimit = surfSlope;

        desiredMovement.Normalize();

        var moveVector = TransformedMovement;

        // Use projected movement input when moving down a sloped surface to prevent bouncing.
        if (ProjectedMovement.y < 0) {
            moveVector = ProjectedMovement;
        }

        var maxVel = isCrouching && !isSliding ? crouchVelocity : maxVelocity;

        velocity += Vector3.ClampMagnitude(moveVector * maxVel - velocity, acceleration * Time.deltaTime);

        JumpMovement();
    }

    private void SurfMovement() {
        if (!wasSurfing) {
            lateralSurfVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude;
            velocity = Vector3.ProjectOnPlane(velocity, groundNormal);
            characterController.slopeLimit = 90f;
        }

        desiredMovement.Normalize();

        // Limit acceleration when going forward
        var moveVector = desiredMovement.SetZ(desiredMovement.z - VelocityDotDirection * desiredMovement.z);
        var newTransformedMovement = transform.TransformDirection(moveVector);
        newTransformedMovement = Vector3.ProjectOnPlane(newTransformedMovement, groundNormal);

        var upwards = Vector3.Dot(groundNormal, Vector3.up);
        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.

        // Acceleration based on input
        var velocityDelta = newTransformedMovement * airAccelaration * upwards * VelocityDotDirection * Time.deltaTime;

        if (limitAirVelocity) {
            // Only add acceleration if we are below the velocity that we started at.
            if (Vector3.Scale(velocity + velocityDelta, new Vector3(1, 0, 1)).magnitude < lateralSurfVelocity) {
                velocity += velocityDelta;
            }
        }
        else {
            velocity += velocityDelta;
        }

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;

        // Air drag / friction.
        velocity -= velocity * airDrag * Time.deltaTime;
    }

    private void AirMovement() {
        if (!wasInAir) {
            lateralJumpVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude;
            characterController.slopeLimit = 90f;
        }

        desiredMovement.Normalize();

        // Limit acceleration when going forward
        var moveVector = desiredMovement.SetZ(desiredMovement.z - VelocityDotDirection * desiredMovement.z);
        var newTransformedMovement = transform.TransformDirection(moveVector);
        var velocityDelta = newTransformedMovement * airAccelaration * VelocityDotDirection * Time.deltaTime;

        if (limitAirVelocity) {
            // Only add acceleration if we are below the velocity that we started at.
            if (Vector3.Scale(velocity + velocityDelta, new Vector3(1, 0, 1)).magnitude < lateralJumpVelocity) {
                velocity += velocityDelta;
            }
        }
        else {
            velocity += velocityDelta;
        }

        // Air drag.
        velocity -= velocity * airDrag * Time.deltaTime;

        // Faster fall velocity.
        if (velocity.y > -fallMaxSpeedUp) {
            velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        }

        // Gravity.
        velocity += Physics.gravity * Time.deltaTime;
    }

    public void AddForce (Vector3 force) {
        velocity += force * Time.deltaTime;
    }
}
