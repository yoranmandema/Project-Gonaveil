using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanWeaponComponent : WeaponComponent {

    private HitScanWeaponComponentProfile profile;

    private void Start () {
        profile = (HitScanWeaponComponentProfile)Profile;
    }

    public override void OnFireStart() {
        var cast = Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, Mathf.Infinity);

        Debug.DrawLine(camera.position, hit.point, Color.red, 10f);

        if (cast) {
            var impactObject = Instantiate(profile.impact, hit.point, Quaternion.LookRotation(Vector3.up, hit.normal));
        }
    }
}
