﻿using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponMovementProfile", menuName = "Weapons/New Weapon Movement Profile")]
public class WeaponMovementProfile : ScriptableObject {
    public float velocityMultiplier = 0.01f;

    public float rotationSpeed = 5f;
    public float rotationAmount = 2;

    public float bobbingAmount = 0.5f;
    public float bobbingEngageTime = 1f;

    public float crouchAngle = 70f;
    public float crouchEngageTime = 0.1f;
    public float crouchEngageSmoothing = 0.05f;

    public float lookDownRetraction = 0.25f;
    public float lookDownSmoothing = 2f;

    public float jumpAmount = 0.25f;
}