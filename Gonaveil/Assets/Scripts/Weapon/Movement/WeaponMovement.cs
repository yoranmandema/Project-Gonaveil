using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public WeaponMovementProfile Profile;

    [HideInInspector] public Vector3 offset;

    private float bobbingLerp = 1f;
    private float bobbingStep = 0f;
    private float crouchingLerp = 0f;
    private float crouchingSmoothedLerp = 0f;
    private float lookDownLerp = 0f;

    private float yaw;
    private float pitch;

    private bool wasInAir;
    private bool wasGrounded;

    void Start()
    {
        offset = transform.localPosition;
    }

    void Update()
    {
        var isGrounded = playerMovement.isGrounded && !playerMovement.isSliding;
        var velocityLerp = playerMovement.velocity.magnitude * Profile.velocityMultiplier;

        bobbingLerp = Mathf.Clamp(bobbingLerp + (isGrounded ? 1 : -1) * Time.deltaTime / Profile.bobbingEngageTime, 0, 1f);
        crouchingLerp = Mathf.Clamp(crouchingLerp + (playerMovement.isCrouching || playerMovement.isSliding ? 1 : -1) * Time.deltaTime / Profile.crouchEngageTime, 0, 1f);
        crouchingSmoothedLerp += (crouchingLerp - crouchingSmoothedLerp) * Time.deltaTime / Profile.crouchEngageSmoothing;
        lookDownLerp += (Mathf.Clamp(transform.forward.y, -1, 0) - lookDownLerp) * Time.deltaTime / Profile.lookDownSmoothing;

        if (isGrounded) bobbingStep += velocityLerp + Time.deltaTime;

        var sideComponent = Vector3.zero;
        var forwardComponent = Vector3.zero;
        var upComponent = Vector3.zero;

        sideComponent += Vector3.right * Mathf.Sin(bobbingStep) * bobbingLerp * velocityLerp;
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        sideComponent += Vector3.right * -crouchingSmoothedLerp * 0.4f;

        forwardComponent += Vector3.forward * lookDownLerp * Profile.lookDownRetraction;

        transform.localPosition = offset + (
            sideComponent +     
            forwardComponent +
            upComponent
            ) * Profile.bobbingAmount;

        yaw += (Input.GetAxis("Mouse X") - yaw) * Time.deltaTime * Profile.rotationSpeed;
        pitch += (-Input.GetAxis("Mouse Y") - pitch) * Time.deltaTime * Profile.rotationSpeed;

        transform.localRotation = Quaternion.Euler(pitch * Profile.rotationAmount, yaw * Profile.rotationAmount, -yaw * Profile.rotationAmount + crouchingSmoothedLerp * Profile.crouchAngle);

        wasInAir = playerMovement.isInAir;
        wasGrounded = isGrounded;
    }
}
