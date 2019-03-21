using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public float velocityMultiplier = 0.05f;
    public float bobbingAmount = 1f;
    public float bobbingEngageTime = 0.25f;
    
    private float bobbingLerp = 1f;
    private float bobbingStep = 0f;
    private Vector3 standardPosition;

    void Start()
    {
        standardPosition = transform.localPosition;
    }

    void Update()
    {
        var isGrounded = playerMovement.isGrounded && !playerMovement.isSliding;
        var velocityLerp = playerMovement.velocity.magnitude * velocityMultiplier;

        bobbingLerp = Mathf.Clamp(bobbingLerp + (isGrounded ? 1 : -1) * Time.deltaTime / bobbingEngageTime, 0, 1f);

        if (isGrounded) bobbingStep += velocityLerp + Time.deltaTime;

        var sideComponent = Vector3.zero;
        var forwardComponent = Vector3.zero;
        var upComponent = Vector3.zero;

        sideComponent += Vector3.right * Mathf.Sin(bobbingStep) * bobbingLerp * velocityLerp;
        forwardComponent += Vector3.forward * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;
        upComponent += Vector3.up * Mathf.Sin(bobbingStep * 2f) * bobbingLerp * 0.25f * velocityLerp;

        transform.localPosition = standardPosition + (
            sideComponent +
            forwardComponent +
            upComponent
            ) * bobbingAmount;
    }
}
