using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBusterWeapon : WeaponSystem {
    public GameObject debugHit;
    public LayerMask shootMask;
    public float range = 100;
    public int burstCount = 3;
    public float burstFireRate = 700;
    public float burstFireCooldown = 5f;

    private float lastBurstTime;

    public override void OnStartPrimary() {
        if (ConsumeFireSample()) {
            if (!ConsumeAmmo()) return;

            FireLine();
        }
    }
    public override void OnStartSecondary() {
        if ((Time.timeSinceLevelLoad - lastBurstTime) > burstFireCooldown) {
            lastBurstTime = Time.timeSinceLevelLoad;

            StartCoroutine(BurstCoroutine());
        }
    }

    private IEnumerator BurstCoroutine () {
        if (ammo == 0) yield return null;

        var i = burstCount;
        var fireTime = 1 / (fireRate / 60);

        while (i > 0 && ConsumeAmmo()) {
            FireLine();

            yield return new WaitForSeconds(fireTime);

            i--;
        }
    }

    private void FireLine () {
        weaponMovement.DoRecoil();

        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit hit, range, shootMask)) {
            Instantiate(debugHit, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
