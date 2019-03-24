using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingRocketProjectile : Projectile {

    public float rotationSpeed = 10f;
    public float fuseTime = 3f;
    public GameObject explosionParticle;
    public float explosionRadius = 1f;
    public float explosionForce = 100f;
    public LayerMask trackingMask;

    private Transform camera;

    public override void OnStart() {
        velocity = transform.forward * startVelocity;

        camera = instigator.transform.Find("Main Camera");

        StartCoroutine(Fuse());
    }

    public override void OnSimulate(ref Vector3 position, float deltaTime) {
        var lookHit = Physics.Raycast(camera.position, camera.forward, out RaycastHit targetPosHit, Mathf.Infinity, trackingMask);

        if (lookHit) {
            var direction = targetPosHit.point - position;

            velocity = Vector3.Slerp(velocity.normalized, direction.normalized, deltaTime * rotationSpeed) * velocity.magnitude;
        } else {
            var direction = camera.forward;

            velocity = Vector3.Slerp(velocity.normalized, direction.normalized, deltaTime * rotationSpeed) * velocity.magnitude;
        }
    }

    public override void OnHit(ref Vector3 position, float deltaTime, RaycastHit hit) {
        Explode();
    }

    private void Explode () {
        GamePlayPhysics.DoExplosion(transform.position, explosionRadius, explosionForce);
        var particle = Instantiate(explosionParticle, transform.position, Quaternion.Euler(0, 0, 0));

        particle.transform.localScale *= 0.25f;

        Destroy(gameObject);
    }

    private IEnumerator Fuse() {
        yield return new WaitForSeconds(fuseTime);

        Explode();
    }
}
