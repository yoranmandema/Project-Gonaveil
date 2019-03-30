using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRifleWeapon : WeaponSystem {

    private void Start () {
        Disable();
    }

    public override void OnStartPrimary() {
        base.OnStartPrimary();

        print("Fired primary");
    }

    public override void OnStartSecondary() {
        base.OnStartSecondary();

        print("Fired secondary");
    }
}
