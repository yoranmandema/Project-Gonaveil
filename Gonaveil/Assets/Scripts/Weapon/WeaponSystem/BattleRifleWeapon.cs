using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRifleWeapon : WeaponSystem {

    public GameObject projectile;
    public float fireRate = 800;
    public float accuracy = 1f;

    private float lastFireTime;

    public override void OnStart () {
        Disable();
    }

    public override void OnStartPrimary() {
        base.OnStartPrimary();
    }

    public override void OnStartSecondary() {
        base.OnStartSecondary();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        if (isFiringPrimary) {
            var fireTime = 1 / (fireRate / 60);

            if (Time.realtimeSinceStartup - lastFireTime > fireTime) {
                Fire();
            }
        }
    }

    private void Fire () {
        lastFireTime = Time.realtimeSinceStartup;

        weaponMovement.DoRecoil();

        var spread = CalculateSpreadVector(accuracy);

        //create bullet
        var projectileObject = Instantiate(projectile, camera.position, camera.rotation) as GameObject;

        var spreadVector = CalculateSpreadVector(accuracy);

        var projectileComponent = projectileObject.GetComponent<Projectile>();

        projectileObject.transform.rotation = Quaternion.LookRotation(spreadVector);
        projectileComponent.barrel = weaponModelData.barrel;
        projectileComponent.instigator = transform.root.gameObject;
        projectileComponent.weaponSystem = this;
        projectileComponent.Fire();
    }
}
