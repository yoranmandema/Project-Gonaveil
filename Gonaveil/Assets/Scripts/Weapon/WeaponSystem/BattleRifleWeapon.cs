using UnityEngine;

public class BattleRifleWeapon : WeaponSystem {

    public AnimationCurve accuracyCurve;

    private float accuracyTime;
    private float accuracy;

    public override void OnStart() {
        base.OnStart();
    }

    public override void OnUpdate() {
        base.OnUpdate();

        var accuracyCurveEnd = accuracyCurve[accuracyCurve.length - 1].time;

        accuracyTime = Mathf.Clamp(accuracyTime + Time.deltaTime * (isFiringPrimary ? 1 : -1), 0, accuracyCurveEnd);

        accuracy = accuracyCurve.Evaluate(accuracyTime);

        if (isFiringPrimary) {
            if (ConsumeFireSample()) {
                if (ConsumeAmmo()) {
                    Fire();
                }
            }
        }
    }

    private void Fire() {
        lastFireTime = Time.realtimeSinceStartup;

        weaponMovement.DoRecoil();

        FireProjectile(accuracy);
    }
}
