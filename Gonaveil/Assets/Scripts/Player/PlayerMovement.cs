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

    private bool canJump = true;
    private bool canJumpCooldown = true;
    private bool wasGrounded;
    private bool wasSurfing;
    private bool wasInAir;

    private Vector3 TransformedMovement => transform.TransformDirection(desiredMovement);
    private Vector3 ProjectedMovement => Vector3.ProjectOnPlane(TransformedMovement, groundNormal).normalized;
    private float GroundSlope => Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up)) * Mathf.Rad2Deg;

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
            if (GroundSlope <= characterController.slopeLimit) {
                isGrounded = true;
            }        
        }

        if (GroundSlope > characterController.slopeLimit) {
            isSurfing = true;
        }

        isInAir = !isGrounded && !isSurfing;
    }

    void Update() {
        GroundCheck();

        characterController.Move(velocity * Time.deltaTime);

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
        if (WantsJumpInput() && canJump && canJumpCooldown) {
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
        if (!wasGrounded) canJump = true;

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
        if (!wasSurfing) velocity = Vector3.ProjectOnPlane(velocity, groundNormal);

        var downVector = Vector3.ProjectOnPlane(Vector3.down, groundNormal);

        Debug.DrawLine(groundPoint, groundPoint + downVector, Color.green);

        velocity += downVector * Time.deltaTime;

        //velocity = velocity.SetY(characterController.velocity.y);
    }

    private void AirMovement () {
        if (!wasInAir) lateralJumpVelocity = Vector3.Scale(velocity, new Vector3(1, 0, 1)).magnitude;

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
