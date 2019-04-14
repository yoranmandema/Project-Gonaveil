using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    public LayerMask mask;
    public float range = 2f;

    void Update() {
        if (InputManager.GetButtonDown("Interact")) {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, range, mask)) {
                var pickup = hit.collider.gameObject.GetComponent<IPickup>();

                if (pickup != null) {
                    pickup.OnPickup(this);
                }
            }
        }
    }
}
