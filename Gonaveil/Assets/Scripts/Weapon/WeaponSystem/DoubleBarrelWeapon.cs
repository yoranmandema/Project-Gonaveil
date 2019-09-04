using UnityEngine;

public class DoubleBarrelWeapon : WeaponSystem {

    public float range = 25f;
    public float spread = 3f;
    public int pellets = 20;

    public float hookDisconnectDistance = 0.25f;
    public float hookJump = 1f;
    public float hookUpForce = 50f;
    public float hookAccel = 100f;
    public float hookVelocity = 3f;
    public float maxHookDistance = 10f;
    public LayerMask hookMask;
    public GameObject impactDebug;

    private bool isUsingGrapplingHook;
    private Vector3 hookPivot;
    private PlayerController playerMovement;

    public override void OnEnable() {
        playerMovement = GetComponentInParent<PlayerController>();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (isUsingGrapplingHook) {
            var direction = (hookPivot - camera.position).normalized;
            var dotVelocity = Vector3.Dot(camera.forward, direction);

            playerMovement.DoAcceleration(direction, hookAccel ,hookVelocity);

            playerMovement.AddForce(Vector3.up * hookUpForce);

            var isInLineOfSight = Physics.Raycast(camera.position, direction, out RaycastHit hit, maxHookDistance, hookMask);

            if ((hit.point - hookPivot).magnitude > hookDisconnectDistance) isInLineOfSight = false;

            if (dotVelocity <= 0 || !isInLineOfSight) {
                isUsingGrapplingHook = false;
            }
        }
    }

    public override void OnFire() {
        weaponMovement.DoRecoil();

        for (var i = 0; i < pellets; i++) {
            var lineCast = FireLine(out RaycastHit hitResult, spread, range);

            if (lineCast) {
                Instantiate(impactDebug, hitResult.point, Quaternion.LookRotation(hitResult.normal));
            }
        }
    }

    public override void OnStartSecondary() {
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxHookDistance, hookMask)) {
            isUsingGrapplingHook = true;

            hookPivot = hit.point;

            if (playerMovement.isGrounded)
                playerMovement.AddForce(Vector3.up * hookJump);
        }
    }

    public override void OnEndSecondary() {
        isUsingGrapplingHook = false;
    }
}
