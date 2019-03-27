using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponMovementProfile", menuName = "Weapons/New Weapon Movement Profile")]
public class WeaponMovementProfile : ScriptableObject {
    public float velocityMultiplier = 0.01f;

    public float rotationSpeed = 5f;
    public float rotationAmount = 2;
    public float rollRotation = 1f;

    public float bobbingSpeed = 75f;
    public float bobbingAmount = 0.5f;
    public float bobbingEngageTime = 1f;
    public float bobbingRotation = 1f;

    public float crouchAngle = 70f;
    public float crouchEngageTime = 0.1f;
    public float crouchEngageSmoothing = 0.05f;
    public float crouchDisturb = 0.1f;

    public float lookDownRetraction = 0.25f;
    public float lookDownSmoothing = 2f;

    public float jumpAmount = 0.25f;

    public float recoil = 10f;
    public float recoilRecovery = 30f;
    public float recoilSide = 0.25f;

    public float wigglePositionAmount = 0.5f;
    public float wiggleAmount = 0.5f;
    public float wiggleForce = 5f;
    public float wiggleDamping = 0.8f;

    public Vector3 offset = new Vector3(0.35f, -0.3f, 0.6f);
}