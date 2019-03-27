using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour {
    public PlayerMovement playerMovement;
    public WeaponMovementProfile profile;

    private float bobbingLerp = 1f;
    private float bobbingStep = 0f;
    private float crouchingLerp = 0f;
    private float crouchingSmoothedLerp = 0f;
    private float crouchChange = 0;
    private float crouchOld = 0;
    private float lookDownLerp = 0f;

    private float yaw;
    private float pitch;

    private Vector3 recoilVector;
    private Vector3 recoilVectorSmoothed;

    private Vector2 swayVector;
    private Vector2 swayForce;

    private Vector3 jiggleVector;
    private Vector3 jiggleForce;

    public void DoRecoil() {
        var recoilAngle = crouchingSmoothedLerp * profile.crouchAngle;

        var x = Mathf.Cos(recoilAngle * Mathf.Deg2Rad);
        var y = Mathf.Sin(recoilAngle * Mathf.Deg2Rad);

        var pitch = profile.recoil;
        var yaw = profile.recoil * Random.Range(-1f, 1f) * profile.recoilSide;

        var usePitch = Mathf.Lerp(yaw, pitch, x);
        var useYaw = Mathf.Lerp(yaw, -pitch, y);

        recoilVector += new Vector3(usePitch, useYaw, profile.recoil);
    }

    void Update() {
        if (profile == null) return;

        var isGrounded = playerMovement.isGrounded && !playerMovement.isSliding;
        var velocityLerp = playerMovement.velocity.magnitude * profile.velocityMultiplier;

        if (isGrounded) bobbingStep += velocityLerp * profile.bobbingSpeed * Time.deltaTime;

        // Positional stuff.

        bobbingLerp = Mathf.Clamp01(bobbingLerp + (isGrounded ? 1 : -1) * Time.deltaTime / profile.bobbingEngageTime);
        lookDownLerp += (Mathf.Clamp(transform.forward.y, -1, 0) - lookDownLerp) * Time.deltaTime / profile.lookDownSmoothing;

        crouchingLerp = Mathf.Clamp01(crouchingLerp + (playerMovement.isCrouching || playerMovement.isSliding ? 1 : -1) * Time.deltaTime / profile.crouchEngageTime);
        crouchingSmoothedLerp += (crouchingLerp - crouchingSmoothedLerp) * Time.deltaTime / profile.crouchEngageSmoothing;
        crouchChange = (crouchingSmoothedLerp - crouchOld) / Time.deltaTime;
        crouchOld = crouchingLerp;

        var sideComponent = Vector3.zero;
        var forwardComponent = Vector3.zero;
        var upComponent = Vector3.zero;

        sideComponent += Vector3.right * Mathf.Sin(bobbingStep) * bobbingLerp * velocityLerp;
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.35f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        sideComponent += Vector3.right * -crouchingSmoothedLerp * 0.2f;

        forwardComponent += Vector3.forward * lookDownLerp * profile.lookDownRetraction;
        forwardComponent += Vector3.forward * recoilVectorSmoothed.z * -0.05f;

        var jiggleDir = (sideComponent + forwardComponent + upComponent) * profile.bobbingAmount;

        jiggleVector *= profile.wiggleDamping;
        jiggleForce += (jiggleDir - jiggleVector) * Time.deltaTime * profile.wiggleForce;
        jiggleVector += jiggleForce;

        var useOffset = Vector3.Lerp(jiggleDir, jiggleVector, profile.wigglePositionAmount);

        transform.localPosition = profile.offset + useOffset;

        // Rotation stuff.

        var bob = Mathf.Sin(bobbingStep) * profile.bobbingRotation * bobbingLerp * velocityLerp;
        var bob2 = Mathf.Sin(bobbingStep * 2f) * profile.bobbingRotation * bobbingLerp * velocityLerp;

        recoilVector += -Vector3.ClampMagnitude(recoilVector, 1) * (profile.recoilRecovery * Time.deltaTime);
        recoilVectorSmoothed += (recoilVector - recoilVectorSmoothed) * Time.deltaTime / 0.04f;

        yaw += (Input.GetAxis("Mouse X") + crouchChange * profile.crouchDisturb + bob - yaw) * Time.deltaTime * profile.rotationSpeed;
        pitch += (-Input.GetAxis("Mouse Y") - crouchChange * profile.crouchDisturb + bob2 - pitch) * Time.deltaTime * profile.rotationSpeed;

        var targetYaw = Input.GetAxis("Mouse X") + crouchChange * profile.crouchDisturb + recoilVectorSmoothed.y + bob;
        var targetPitch = Input.GetAxis("Mouse Y") + crouchChange * profile.crouchDisturb + recoilVectorSmoothed.x + bob2;

        var swayDir = new Vector2(-targetPitch, targetYaw) * profile.rotationAmount;

        swayVector *= profile.wiggleDamping;
        swayForce += (swayDir - swayVector) * Time.deltaTime * profile.wiggleForce;
        swayVector += swayForce;

        var useYaw = Mathf.Lerp(yaw, swayVector.y, profile.wiggleAmount);
        var usePitch = Mathf.Lerp(pitch - recoilVectorSmoothed.x, swayVector.x, profile.wiggleAmount);

        transform.localRotation = Quaternion.Euler(
            usePitch, 
            useYaw, 
            -useYaw * profile.rollRotation + crouchingSmoothedLerp * profile.crouchAngle
            );
    }
}
