using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : Projectile
{
    public float fuseTime = 3f;
    public float explosionRadius = 3f;
    public float explosionForce = 500f;

    public override void OnStart() {
        velocity = transform.forward * startVelocity;

        velocity += instigator.GetComponent<CharacterController>().velocity; 
    }

    public override void OnHit(RaycastHit hit, ref Vector3 position, float deltaTime) {
        velocity = Vector3.Reflect(velocity.normalized, hit.normal) * velocity.magnitude;

        position = hit.point + velocity * deltaTime;
    }

    private IEnumerator Fuse () {
        yield return new WaitForSeconds(fuseTime);

        GamePlayPhysics.DoExplosion(transform.position, explosionRadius, explosionForce);

        Destroy(gameObject);
    }
}
