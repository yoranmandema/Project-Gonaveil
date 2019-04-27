using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public bool IsAlive => health > 0;

    public float respawnTime = 3f;
    public int health = 100;

    private PlayerMovement playerMovement;

    private void Start () {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void DealDamage (int amount) {
        health = Mathf.Max(0, health - amount);

        if (health == 0) {
            Die();
        }
    }

    public void Die () {
        playerMovement.allowInput = false;

        StartCoroutine(RespawnTimer());
    }

    public void Spawn () {
        health = 100;

        playerMovement.allowInput = true;
    }

    private IEnumerator RespawnTimer () {
        yield return new WaitForSeconds(respawnTime);

        Spawn();
    }
}
