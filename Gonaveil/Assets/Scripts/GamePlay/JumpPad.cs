using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {

    public float height;
    public float time;

    private void OnTriggerEnter(Collider collider) {
        var playerMovement = collider.GetComponent<PlayerMovement>();

        if (playerMovement != null) {
            StartCoroutine(ApplyForce(playerMovement));
        }
    }

    private IEnumerator ApplyForce (PlayerMovement playerMovement) {
        var relativeHeight = 0f;

        while (relativeHeight < 1) {
            relativeHeight = (playerMovement.transform.position.y - transform.position.y) / height;

            playerMovement.velocity = playerMovement.velocity.SetY((1 / time * height) * Mathf.Pow(Mathf.Max(1 - relativeHeight,0.01f), 0.5f));

            yield return null;
        }

        //playerMovement.velocity = playerMovement.velocity.SetY(1f).normalized * 5f;
    }
}
