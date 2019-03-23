using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : Projectile {
    public GameObject impactObject;

    public override void OnHit(RaycastHit hit, ref Vector3 position, float deltaTime) {
        //force bullet effect to centre
        effect.localPosition = Vector3.zero;
        //create impact
        Instantiate(impactObject, hit.point, Quaternion.LookRotation(Vector3.up, hit.normal), hit.transform);
        //destroy bullet
        Destroy(gameObject);
    }
}
