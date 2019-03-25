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

    void TestListen(float charge) {
        recoilVector += new Vector3(profile.recoil, profile.recoil * Random.Range(-1f, 1f) * profile.recoilSide, profile.recoil);
    }

    void Start() {
        EventManager.StartListening("Shot Fired", TestListen);
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
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        sideComponent += Vector3.right * -crouchingSmoothedLerp * 0.2f;

        forwardComponent += Vector3.forward * lookDownLerp * profile.lookDownRetraction;
        forwardComponent += Vector3.forward * recoilVectorSmoothed.z * -0.05f;

        transform.localPosition = profile.offset + (
            sideComponent +
            forwardComponent +
            upComponent
            ) * profile.bobbingAmount;

        // Rotation stuff.

        recoilVector += -Vector3.ClampMagnitude(recoilVector, 1) * (profile.recoilRecovery * Time.deltaTime);
        recoilVectorSmoothed += (recoilVector - recoilVectorSmoothed) * Time.deltaTime / 0.04f;

        yaw += (Input.GetAxis("Mouse X") + crouchChange * profile.crouchDisturb - yaw) * Time.deltaTime * profile.rotationSpeed;
        pitch += (-Input.GetAxis("Mouse Y") - crouchChange * profile.crouchDisturb - pitch) * Time.deltaTime * profile.rotationSpeed;

        var targetYaw = Input.GetAxis("Mouse X") + crouchChange * profile.crouchDisturb + recoilVectorSmoothed.y;
        var targetPitch = Input.GetAxis("Mouse Y") + crouchChange * profile.crouchDisturb + recoilVectorSmoothed.x;

        var swayDir = new Vector2(-targetPitch, targetYaw) * profile.rotationAmount;

        swayVector *= profile.wiggleDamping;
        swayForce += (swayDir - swayVector) * Time.deltaTime * profile.wiggleForce;
        swayVector += swayForce;

        var useYaw = Mathf.Lerp(yaw, swayVector.y, profile.wiggleAmount);
        var usePitch = Mathf.Lerp(pitch - recoilVectorSmoothed.x, swayVector.x, profile.wiggleAmount);

        transform.localRotation = Quaternion.Euler(
            usePitch, 
            useYaw, 
            -useYaw * profile.rotationAmount + crouchingSmoothedLerp * profile.crouchAngle
            );
    }
}
