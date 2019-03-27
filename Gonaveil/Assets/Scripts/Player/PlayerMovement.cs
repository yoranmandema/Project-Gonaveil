using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    public Transform cameraTransform;
    public float cameraHeight = 1.8f;

    public float maxVelocity = 12f;
    public float acceleration = 200f;
    public float stepSlope = 85f;

    public float airAcceleration = 150f;
    public float airDrag = 0.05f;
    public float airVelocityMultiplier = 1.05f;
    public bool limitAirVelocity = false;
    public float fallSpeedMultiplier = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public float jumpHeight = 1f;
    public float jumpLateralSpeedMultiplier = 1.1f;
    public bool autoJump;
    public float jumpCooldown = 0.25f;
    public float upHillJumpBoost = 5f;

    public float surfSlope = 45f;
    public float surfAcceleration = 150f;

    public float crouchHeight = 1f;
    public float crouchTime = 0.5f;
    public float crouchVelocity = 8f;
    public float crouchCameraHeight = 0.9f;

    public float slideBoost = 1.25f;
    public float slideVelocityThreshold = 10f;

    public float flipTime = 0.25f;

    public bool isFlipped;
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
    private Vector3 flipAxis;
    private Rigidbody rb;
    private LayerMask groundMask;
    private float maxAirVelocity;
    private float appliedCrouchHeight = 2f;
    private float crouchLerp = 1f;
    private float desiredCrouchLerp = 1f;

    private bool canJump = true;
    private bool canJumpCooldown = true;
    private bool wasGrounded;
    private bool wasSurfing;
    private bool wasInAir;

    private bool isEndingFlip;

    private Vector3 TransformedMovement => transform.TransformDirection(desiredMovement);
    private Vector3 ProjectedMovement => Vector3.ProjectOnPlane(TransformedMovement, groundNormal).normalized;
    private float GroundSlope => Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up)) * Mathf.Rad2Deg;
    private float VelocityDotDirection => Mathf.Max(0, Vector3.Dot(transform.forward, Vector3.Scale(velocity.normalized, new Vector3(1, 0, 1))));

    private void Start() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();

        groundMask = ((LayerMask)gameObject.layer).GetReverseLayerMask();
        groundMask ^= 1 << gameObject.layer;

        groundMask ^= 1 << LayerMask.NameToLayer("Ignore Raycast");
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

    private IEnumerator FlipStart () {
        isFlipped = true;

        flipAxis = transform.right;
        var flipStep = 0f;

        while (flipStep < 1) {
            flipStep += Time.deltaTime / flipTime;

            transform.RotateAround(transform.TransformPoint(characterController.center), flipAxis, 180 * Time.deltaTime / flipTime);

            yield return new WaitForEndOfFrame();
        }

        var forwardRelativeToSurfaceNormal = Vector3.Cross(transform.right, -Vector3.up);
        var targetRotation = Quaternion.LookRotation(forwardRelativeToSurfaceNormal, -Vector3.up);

        transform.rotation = targetRotation;
    }

    private IEnumerator FlipEnd() {
        if (!isFlipped) yield break;
        if (isEndingFlip) yield break;

        isEndingFlip = true;
        var flipStep = 0f;

        while (flipStep < 1) {
            flipStep += Time.deltaTime / flipTime;

            transform.RotateAround(transform.TransformPoint(characterController.center), flipAxis, 180 * Time.deltaTime / flipTime);

            yield return new WaitForEndOfFrame();
        }

        var forwardRelativeToSurfaceNormal = Vector3.Cross(transform.right, Vector3.up);
        var targetRotation = Quaternion.LookRotation(forwardRelativeToSurfaceNormal, Vector3.up);

        transform.rotation = targetRotation;

        isFlipped = false;
        isEndingFlip = false;
    }

    private void GroundCheck() {
        if (wasInAir && !isInAir) {
            StartCoroutine(FlipEnd());
        }

        wasSurfing = isSurfing;
        wasGrounded = isGrounded;
        wasInAir = isInAir;

        isGrounded = characterController.isGrounded;
        isSurfing = false;

        var castPosition = transform.position + transform.up * characterController.radius;
        var castDirection = -transform.up;

        if (transform.up.y < 0) {
            castPosition = transform.position + transform.up * (characterController.height - characterController.radius);
            castDirection = transform.up;
        }

        var groundHits = Physics.SphereCastAll(
            castPosition,
            characterController.radius,
            castDirection,
            0.1f,
            groundMask
            );

        var normal = Vector3.zero;
        var hitPoint = Vector3.zero;

        foreach (var hit in groundHits) {
            var rayCast = Physics.Raycast(hit.point + hit.normal * 0.01f, -hit.normal, out RaycastHit rayCastHit, 0.02f, groundMask);

            var useUpwards = (rayCastHit.normal != hit.normal) && (Mathf.Acos(rayCastHit.normal.y) * Mathf.Rad2Deg > stepSlope);

            if (useUpwards) {
                normal += Vector3.up;
            } else {
                normal += hit.normal;
            }

            hitPoint += hit.point;
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
                velocity *= slideBoost;
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

        desiredMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

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
            var jumpVelocity = Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude * (fallSpeedMultiplier - 1));

            // Boost the jump when going uphill.
            if (ProjectedMovement.y > 0) {
                jumpVelocity += Vector3.up * lateralVelocity.magnitude * upHillJumpBoost * (1 - Vector3.Dot(groundNormal, Vector3.up));
            }

            velocity = jumpVelocity + lateralVelocity;
        }
    }

    private void GroundMovement() {
        if (!wasGrounded) {
            characterController.slopeLimit = surfSlope;
        }

        var moveVector = TransformedMovement;

        // Use projected movement input when moving down a sloped surface to prevent bouncing.
        //if (ProjectedMovement.y < 0) {
            moveVector = ProjectedMovement;
        //}

        var maxVel = isCrouching && !isSliding ? crouchVelocity : maxVelocity;

        velocity += Vector3.ClampMagnitude(moveVector * maxVel - velocity, acceleration * Time.deltaTime);

        JumpMovement();
    }

    private void SurfMovement() {
        if (!wasSurfing) {
            //maxAirVelocity = Mathf.Max(0.01f, velocity.SetY(0).magnitude * airVelocityMultiplier);
            maxAirVelocity = maxVelocity;
            velocity = Vector3.ProjectOnPlane(velocity, groundNormal);

            //Debug.DrawLine(groundPoint, groundPoint + velocity, Color.yellow, 100f);
        
            characterController.slopeLimit = 90f;
        }

        var upwards = Vector3.Dot(groundNormal, Vector3.up);
        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.

        DoAirAcceleration(surfAcceleration);

        // Air drag / friction.
        velocity -= velocity * airDrag * Time.deltaTime;

        Debug.DrawLine(groundPoint, groundPoint + downVector, Color.cyan);

        // Faster fall velocity.
        if (velocity.y > -fallMaxSpeedUp) velocity += downVector * -Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;
    }

    private void AirMovement() {
        if (!wasInAir) {
            maxAirVelocity = Mathf.Max(0.01f, velocity.SetY(0).magnitude * airVelocityMultiplier);

            characterController.slopeLimit = 90f;
        }

        DoAirAcceleration(airAcceleration);

        // Air drag.
        velocity -= velocity * airDrag * Time.deltaTime;

        // Faster fall velocity.
        if (velocity.y > -fallMaxSpeedUp) velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;

        // Gravity.
        velocity += Physics.gravity * Time.deltaTime;

        if (Input.GetButtonDown("Flip")) StartCoroutine(FlipStart());
        if (Input.GetButtonUp("Flip") && isFlipped) StartCoroutine(FlipEnd());
    }

    private void DoAirAcceleration (float maxAccel) {
        var velocityDelta = GetAirAcceleration(transform.TransformDirection(desiredMovement), maxAccel);

        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal); // Vector going down the ramp.
        var groundDot = 1 - (Vector3.Dot(velocityDelta.normalized, -downVector) + 1) / 2;

        velocityDelta *= groundDot;

        if (limitAirVelocity) {
            var deltaAmount = Mathf.Clamp01((maxAirVelocity - (velocity - velocityDelta).magnitude) / maxAirVelocity);

            velocity += velocityDelta;

            var y = velocity.y;

            velocity = Vector3.ClampMagnitude(velocity.SetY(0), maxAirVelocity);

            velocity.y = y;
        }
        else {
            velocity += velocityDelta;
        }
    }

    private Vector3 GetAirAcceleration (Vector3 wishDirection, float maxAccel) {
        var dotVelocity = Vector3.Dot(velocity.SetY(0), wishDirection);
        var addSpeed = maxVelocity - dotVelocity;
        addSpeed = Mathf.Clamp(addSpeed, 0, maxAccel * Time.deltaTime);

        return wishDirection * addSpeed;
    }

    public void AddForce (Vector3 force) {
        velocity += force * Time.deltaTime;
    }
}
