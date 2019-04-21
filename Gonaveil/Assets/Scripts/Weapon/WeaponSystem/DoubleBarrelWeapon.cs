using UnityEngine;

public class DoubleBarrelWeapon : WeaponSystem {

    public float spread = 3f;
    public int pellets = 20;

    public float hookDamping = 0.25f;
    public float hookForce = 100f;
    public float hookVelocity = 3f;
    public float maxHookDistance = 10f;
    public LayerMask hookMask;

    private bool isUsingGrapplingHook;
    private Vector3 hookPivot;
    private PlayerMovement playerMovement;

    public override void OnEnable() {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (isUsingGrapplingHook) {
            var direction = (hookPivot - playerMovement.transform.position).normalized;
            var dotVelocity = Vector3.Dot(camera.forward, direction);

            playerMovement.AddForce(
                direction * hookForce * dotVelocity + Vector3.up * 5f
                );

            if (dotVelocity <= 0) {
                isUsingGrapplingHook = false;
            }
        }
    }

    public override void OnStartPrimary() {
        //if (ConsumeFireSample()) {
        //    if (!ConsumeAmmo()) return;

        //    weaponMovement.DoRecoil();

        //    for (var i = 0; i < pellets; i++) {
        //        FireProjectile(spread);
        //    }
        //}
    }

    public override void OnStartSecondary() {
        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, maxHookDistance, hookMask)) {
            isUsingGrapplingHook = true;

            hookPivot = hit.point;

            if (playerMovement.isGrounded)
                playerMovement.AddForce(Vector3.up * 1000f);
        }
    }

    public override void OnEndSecondary() {
        isUsingGrapplingHook = false;
    }
}
