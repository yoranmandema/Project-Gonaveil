using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{

    public InventorySystem inventory;
    public UnityEngine.UI.Text currentWeaponText;
    public UnityEngine.UI.Text holsteredWeaponText;

    private bool disabled;

    // Its probably stupid to put all of this UI logic inside Update but this is temporary anyway.
    private void Update() {
        if (!inventory.HasAnyWeapons && !disabled) {
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);

            disabled = true;
        } else if (inventory.HasAnyWeapons && disabled) {
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);

            disabled = false;
        }

        if (disabled) return;

        currentWeaponText.text = inventory.CurrentWeapon.weaponParameters.weaponName;
    }
}
