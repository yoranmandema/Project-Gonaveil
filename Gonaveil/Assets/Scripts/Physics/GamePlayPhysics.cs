using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayPhysics
{
    
    public enum ExplosionType : byte {
        Players = 1,
        Projectiles = 2,
        PlayersAndProjectiles = Projectiles | Players
    }

    private static LayerMask GetExplosionTypeLayerMask (ExplosionType explosionType) {
        switch (explosionType) {
            case ExplosionType.PlayersAndProjectiles:
                return LayerMask.GetMask("Player","Projectile");
            case ExplosionType.Players:
                return LayerMask.NameToLayer("Player");
            default:
                return 0;
        }
    }

    public static void DoExplosion (Vector3 position, float radius, float force, float upwards = 1f, ExplosionType explosionType = ExplosionType.Players) {
        var layerMask = GetExplosionTypeLayerMask(explosionType);
        var mask = (1 << 0) | (1 << layerMask);

        var colliders = Physics.OverlapSphere(position, radius, mask);

        foreach (Collider hit in colliders) {
            var closestPoint = hit.ClosestPoint(position);
            var direction = position - closestPoint;

            var rayCast = Physics.Raycast(closestPoint, direction.normalized, direction.magnitude - 0.025f, mask);

            if (rayCast) {
                //Debug.DrawLine(closestPoint, closestPoint + (direction.normalized * (direction.magnitude - 0.025f)), Color.red, 100f);
                continue;
            }

            //Debug.DrawLine(closestPoint, closestPoint + (direction.normalized * (direction.magnitude - 0.025f)), Color.yellow, 100f);

            // Should the explosion affect players?
            if ((explosionType & ExplosionType.Players) != 0) {
                var playerMovement = hit.GetComponent<PlayerMovement>();

                if (playerMovement != null) {
                    var difference = hit.transform.position - position;

                    var addForce = difference.normalized * force + Vector3.up * force * upwards;

                    playerMovement.AddForce(addForce);

                    continue;
                }
            }

            var rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(force, position, radius, upwards);
        }
    }
}
