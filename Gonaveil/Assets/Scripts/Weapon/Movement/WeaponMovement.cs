using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public WeaponMovementProfile profile;

    private float bobbingLerp = 1f;
    private float bobbingStep = 0f;
    private float crouchingLerp = 0f;
    private float crouchingSmoothedLerp = 0f;
    private float lookDownLerp = 0f;

    private float yaw;
    private float pitch;

    private float recoil;
    private float recoilSmoothed;

    void TestListen(float charge) {
        recoil += profile.recoil;
    }

    void Start () {
        EventManager.StartListening("Shot Fired", TestListen);
    }

    void Update()
    {
        if (profile == null) return;

        var isGrounded = playerMovement.isGrounded && !playerMovement.isSliding;
        var velocityLerp = playerMovement.velocity.magnitude * profile.velocityMultiplier;

        bobbingLerp = Mathf.Clamp(bobbingLerp + (isGrounded ? 1 : -1) * Time.deltaTime / profile.bobbingEngageTime, 0, 1f);
        crouchingLerp = Mathf.Clamp(crouchingLerp + (playerMovement.isCrouching || playerMovement.isSliding ? 1 : -1) * Time.deltaTime / profile.crouchEngageTime, 0, 1f);
        crouchingSmoothedLerp += (crouchingLerp - crouchingSmoothedLerp) * Time.deltaTime / profile.crouchEngageSmoothing;
        lookDownLerp += (Mathf.Clamp(transform.forward.y, -1, 0) - lookDownLerp) * Time.deltaTime / profile.lookDownSmoothing;

        if (isGrounded) bobbingStep += velocityLerp * profile.bobbingSpeed * Time.deltaTime;

        var sideComponent = Vector3.zero;
        var forwardComponent = Vector3.zero;
        var upComponent = Vector3.zero;

        sideComponent += Vector3.right * Mathf.Sin(bobbingStep) * bobbingLerp * velocityLerp;
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        sideComponent += Vector3.right * -crouchingSmoothedLerp * 0.2f;

        forwardComponent += Vector3.forward * lookDownLerp * profile.lookDownRetraction;
        forwardComponent += Vector3.forward * recoilSmoothed * -0.05f;

        transform.localPosition = profile.offset + (
            sideComponent +     
            forwardComponent +
            upComponent
            ) * profile.bobbingAmount;

        yaw += (Input.GetAxis("Mouse X") - yaw) * Time.deltaTime * profile.rotationSpeed;
        pitch += (-Input.GetAxis("Mouse Y") - pitch) * Time.deltaTime * profile.rotationSpeed;

        transform.localRotation = Quaternion.Euler(pitch * profile.rotationAmount + -recoilSmoothed, yaw * profile.rotationAmount, -yaw * profile.rotationAmount + crouchingSmoothedLerp * profile.crouchAngle);

        recoil = Mathf.Max(recoil - profile.recoilRecovery * Time.deltaTime, 0);
        recoil = Mathf.Min(recoil, 10f);

        recoilSmoothed += (recoil - recoilSmoothed) * Time.deltaTime / 0.04f;
    }
}
