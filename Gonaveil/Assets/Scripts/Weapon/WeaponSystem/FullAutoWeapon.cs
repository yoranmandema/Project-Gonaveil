using UnityEngine;

public class FullAutoWeapon : WeaponSystem {

    public override void OnUpdate() {
        base.OnUpdate();

        if (isFiringPrimary) {
            if (ConsumeFireSample()) {
                if (ConsumeAmmo()) {
                    Fire();
                }
            }
        }
    }

    private void Fire() {
        weaponMovement.DoRecoil();

        FireProjectile(accuracy);
    }
}
