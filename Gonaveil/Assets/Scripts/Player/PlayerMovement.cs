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
    public float jumpCooldown = 0.25f;
    public float jumpLateralSpeedMultiplier = 1.1f;
    public float upHillJumpBoost = 5f;

    public float surfSlope = 45f;

    public bool limitAirVelocity = false;
    public float fallSpeedMultiplier = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public bool isGrounded;
    public bool isInAir;
    public bool isSurfing;

    private CharacterController characterController;
    private Vector3 groundNormal;
    private Vector3 groundPoint;
    private Vector3 velocity;
    private Vector3 desiredMovement;
    private Rigidbody rb;
    private LayerMask groundMask;
    private float lateralJumpVelocity;
    private float lateralSurfVelocity;

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

        groundMask = ((LayerMask)gameObject.layer).GetReverseLayerMask();
        groundMask ^= 1 << gameObject.layer;
    }

    private bool WantsJumpInput () {
        if (autoJump) {
            return Input.GetButton("Jump");
        } else {
            return Input.GetButtonDown("Jump");
        }
    }

    private IEnumerator JumpCooldown () {
        canJumpCooldown = false;

        yield return new WaitForSeconds(jumpCooldown);

        canJumpCooldown = true;
    }

    private void GroundCheck () {
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
        } else {
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
    }

    void Update() {
        GroundCheck();

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
        else if (isGrounded) {
            GroundMovement();
        }
        else {
            AirMovement();
        }
    }

    private void JumpMovement () {
        if (WantsJumpInput() && canJumpCooldown) {
            canJump = false;

            StartCoroutine(JumpCooldown());

            var lateralVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)) * jumpLateralSpeedMultiplier;
            var jumpVelocity =
                Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude);

            // Boost the jump when going uphill.
            if (ProjectedMovement.y > 0) {
                jumpVelocity += Vector3.up * lateralVelocity.magnitude * upHillJumpBoost * (1 - Vector3.Dot(groundNormal, Vector3.up));
            }

            velocity = jumpVelocity + lateralVelocity;
        }
    }

    private void GroundMovement () {
        if (!wasGrounded) characterController.slopeLimit = surfSlope;

        desiredMovement.Normalize();

        var moveVector = TransformedMovement;

        // Use projected movement input when moving down a sloped surface to prevent bouncing.
        if (ProjectedMovement.y < 0) {
            moveVector = ProjectedMovement;
        }

        velocity += Vector3.ClampMagnitude(moveVector * maxVelocity - velocity, acceleration * Time.deltaTime);

        JumpMovement();
    }

    private void SurfMovement () {
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
        } else {
            velocity += velocityDelta;
        }

        // Gravity.
        velocity += downVector * -Physics.gravity.y * upwards * Time.deltaTime;

        // Air drag / friction.
        velocity -= velocity * airDrag * Time.deltaTime;
    }

    private void AirMovement () {
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
        if (velocity.y < 0 && velocity.y > -fallMaxSpeedUp) {
            velocity += Vector3.up * Physics.gravity.y * (fallSpeedMultiplier - 1) * Time.deltaTime;
        }

        // Gravity.
        velocity += Physics.gravity * Time.deltaTime;
    }
}
