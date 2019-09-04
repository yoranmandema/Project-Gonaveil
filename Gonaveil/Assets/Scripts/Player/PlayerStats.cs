using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public bool IsAlive => health > 0;

    public float respawnTime = 3f;
    public int health = 100;

    public bool isRespawning = false;

    private PlayerController playerMovement;

    private void Start () {
        playerMovement = GetComponent<PlayerController>();
    }

    private void Update () {
        if (health == 0 && !isRespawning) {
            Die();
        }
    }

    public void DealDamage (int amount) {
        health = Mathf.Max(0, health - amount);

        if (health == 0 && !isRespawning) {
            Die();
        }
    }

    public void Die () {
        //playerMovement.allowInput = false;
        isRespawning = true;

        StartCoroutine(RespawnTimer());
    }

    public void Spawn () {
        health = 100;
        isRespawning = false;

        //playerMovement.allowInput = true;

        var spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        var spawn = spawns[Random.Range(0, spawns.Length)];

        transform.position = spawn.transform.position;
    }

    private IEnumerator RespawnTimer () {
        yield return new WaitForSeconds(respawnTime);

        Spawn();
    }
}
