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

    public static void DoExplosion (Vector3 position, float radius, float force, float upwards, ExplosionType explosionType = ExplosionType.Players) {
        var layerMask = GetExplosionTypeLayerMask(explosionType);

        var colliders = Physics.OverlapSphere(position, radius, (1 << 0) | (1 << layerMask));
        
        foreach (Collider hit in colliders) {
            // Should the explosion affect players?
            if ((explosionType & ExplosionType.Players) != 0) {
                var playerMovement = hit.GetComponent<PlayerMovement>();

                if (playerMovement != null) {
                    var difference = hit.transform.position - position;

                    var addForce = difference.normalized * force;

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
