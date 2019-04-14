using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBarrelWeapon : WeaponSystem {

    public GameObject projectile;
    public float spread = 3f;
    public float fireRate = 400f;
    public int pellets = 20;

    public float hookDamping = 0.25f;
    public float hookForce = 100f;
    public float hookVelocity = 3f;
    public float maxHookDistance = 10f;
    public LayerMask hookMask;

    private float lastFireTime;
    private bool isUsingGrapplingHook;
    private Vector3 hookPivot;
    private PlayerMovement playerMovement;

    public override void OnStart() {
        Disable();

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
        var fireTime = 1 / (fireRate / 60);

        if ((Time.realtimeSinceStartup - lastFireTime) > fireTime) {
            weaponMovement.DoRecoil();

            lastFireTime = Time.realtimeSinceStartup;

            for (var i = 0; i < pellets; i++) {
                //create bullet
                var projectileObject = Instantiate(projectile, camera.position, camera.rotation) as GameObject;

                var spreadVector = CalculateSpreadVector(spread);

                var projectileComponent = projectileObject.GetComponent<Projectile>();

                projectileObject.transform.rotation = Quaternion.LookRotation(spreadVector);
                projectileComponent.barrel = weaponModelData.barrel;
                projectileComponent.instigator = transform.root.gameObject;
                projectileComponent.weaponSystem = this;
                projectileComponent.Fire();
            }
        }
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
