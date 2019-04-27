using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {
    private void OnTriggerEnter (Collider collider) {
        var playerStats = collider.GetComponent<PlayerStats>();

        if (playerStats != null) {
            playerStats.Die();
        }
    }
}
