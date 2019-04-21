using UnityEngine;

public class FullAutoWeapon : WeaponSystem {

    public override void OnFire() {
        weaponMovement.DoRecoil();

        FireProjectile(accuracy);
    }
}
