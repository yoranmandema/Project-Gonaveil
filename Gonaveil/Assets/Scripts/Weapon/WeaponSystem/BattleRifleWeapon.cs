using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRifleWeapon : WeaponSystem {

    public GameObject projectile;
    public float fireRate = 800;
    public AnimationCurve accuracyCurve; 

    private float lastFireTime;
    private float accuracyTime;
    private float accuracy;

    public override void OnStart () {
        Disable();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        var accuracyCurveEnd = accuracyCurve[accuracyCurve.length - 1].time;

        accuracyTime = Mathf.Clamp(accuracyTime + Time.deltaTime * (isFiringPrimary ? 1 : -1), 0, accuracyCurveEnd);

        accuracy = accuracyCurve.Evaluate(accuracyTime);

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
