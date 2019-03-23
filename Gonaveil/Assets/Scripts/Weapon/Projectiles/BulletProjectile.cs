using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : Projectile {
    public GameObject impactObject;

    public override void OnHit(ref Vector3 position, float deltaTime, RaycastHit hit) {
        Instantiate(impactObject, hit.point, Quaternion.LookRotation(Vector3.up, hit.normal), hit.transform);

        Destroy(gameObject);
    }
}
