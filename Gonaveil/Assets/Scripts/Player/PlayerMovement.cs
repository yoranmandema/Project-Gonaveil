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

    public float fallSpeedMultiplier = 1.5f;
    public float fallMaxSpeedUp = 10f;

    public bool isOnWalkableGround;
    public bool isGrounded;
    public float maxGroundInclination = 0.75f;

    private CharacterController characterController;
    private Vector3 groundNormal;
    private Vector3 groundPoint;
    private Vector3 velocity;
    private Vector3 desiredMovement;
    private Rigidbody rb;
    private LayerMask groundMask;
    private float lateralJumpVelocity;

    private bool canJump = true;
    private bool canJumpCooldown = true;
    private bool wasGrounded;

    private Vector3 TransformedMovement => transform.TransformDirection(desiredMovement);
    private Vector3 ProjectedMovement => Vector3.ProjectOnPlane(TransformedMovement, groundNormal).normalized;

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
        wasGrounded = isGrounded;

        isGrounded = characterController.isGrounded;

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
            if ((Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up)) * Mathf.Rad2Deg) <= characterController.slopeLimit) {
                isGrounded = true;
            }        
        }
    }

    void Update() {
        GroundCheck();

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

    private void JumpMovement () {
        if (WantsJumpInput() && canJump && canJumpCooldown) {
            canJump = false;

            StartCoroutine(JumpCooldown());

            var lateralVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)) * jumpLateralSpeedMultiplier;
            var jumpVelocity =
                Vector3.up * Mathf.Sqrt(jumpHeight * 2f * Physics.gravity.magnitude);

            // Boost the jump when going uphill.
            if (ProjectedMovement.y > 0) {
                jumpVelocity += groundNormal * lateralVelocity.magnitude * upHillJumpBoost * (1 - Vector3.Dot(groundNormal, Vector3.up));
            }

            velocity = jumpVelocity + lateralVelocity;

            lateralJumpVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude;
        }
    }

    private void GroundMovement () {
        desiredMovement.Normalize();

        var moveVector = TransformedMovement;

        // Use projected movement input when moving down a sloped surface to prevent bouncing.
        if (ProjectedMovement.y < 0) {
            moveVector = ProjectedMovement;
        }

        velocity += Vector3.ClampMagnitude(moveVector * maxVelocity - velocity, acceleration * Time.deltaTime);

        JumpMovement();
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
