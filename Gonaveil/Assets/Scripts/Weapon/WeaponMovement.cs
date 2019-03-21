using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public float velocityMultiplier = 0.05f;

    public float rotationSpeed = 0.5f;
    public float rotationAmount = 2.5f;

    public float bobbingAmount = 1f;
    public float bobbingEngageTime = 0.25f;

    public float crouchAngle = 75f;
    public float crouchEngageTime = 0.1f;
    public float crouchEngageSmoothing = 0.05f;
    
    public float jumpAmount = 0.25f;

    private float bobbingLerp = 1f;
    private float bobbingStep = 0f;
    private float crouchingLerp = 0f;
    private float crouchingSmoothedLerp = 0f;

    private Vector3 standardPosition;

    private float yaw;
    private float pitch;

    private bool wasInAir;
    private bool wasGrounded;

    void Start()
    {
        standardPosition = transform.localPosition;
    }

    void Update()
    {
        var isGrounded = playerMovement.isGrounded && !playerMovement.isSliding;
        var velocityLerp = playerMovement.velocity.magnitude * velocityMultiplier;

        bobbingLerp = Mathf.Clamp(bobbingLerp + (isGrounded ? 1 : -1) * Time.deltaTime / bobbingEngageTime, 0, 1f);
        crouchingLerp = Mathf.Clamp(crouchingLerp + (playerMovement.isCrouching || playerMovement.isSliding ? 1 : -1) * Time.deltaTime / crouchEngageTime, 0, 1f);

        crouchingSmoothedLerp += (crouchingLerp - crouchingSmoothedLerp) * Time.deltaTime / crouchEngageSmoothing;

        if (isGrounded) bobbingStep += velocityLerp + Time.deltaTime;

        var sideComponent = Vector3.zero;
        var forwardComponent = Vector3.zero;
        var upComponent = Vector3.zero;

        sideComponent += Vector3.right * Mathf.Sin(bobbingStep) * bobbingLerp * velocityLerp;
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        sideComponent += Vector3.right * -crouchingSmoothedLerp * 0.4f;

        transform.localPosition = standardPosition + (
            sideComponent +
            forwardComponent +
            upComponent
            ) * bobbingAmount;

        yaw += (Input.GetAxis("Mouse X") - yaw) * Time.deltaTime * rotationSpeed;
        pitch += (-Input.GetAxis("Mouse Y") - pitch) * Time.deltaTime * rotationSpeed;

        //if (wasInAir != playerMovement.isInAir || wasGrounded != isGrounded) {
        //    pitch += playerMovement.velocity.y * jumpAmount;
        //}

        transform.localRotation = Quaternion.Euler(pitch * rotationAmount, yaw * rotationAmount, crouchingSmoothedLerp * crouchAngle);

        wasInAir = playerMovement.isInAir;
        wasGrounded = isGrounded;
    }
}
