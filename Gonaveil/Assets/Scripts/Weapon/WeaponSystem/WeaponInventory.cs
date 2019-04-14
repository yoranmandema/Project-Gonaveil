using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour {
    public GameObject primary;
    public GameObject secondary;
    public GameObject grenade;
    public Transform weaponHolder;

    private int index;

    public GameObject CurrentWeapon {
        get {
            switch (index) {
                case 0:
                    return primary;
                case 1:
                    return secondary;
                case 2:
                    return grenade;
                default:
                    return null;
            }
        }

        private set {
            switch (index) {
                case 0:
                    primary = value;
                    break;
                case 1:
                    secondary = value;
                    break;
                case 2:
                    grenade = value;
                    break;
                default:
                    break;
            }
        }
    }

    public bool HasAnyWeapons {
        get {
            var hasAnyWeapon = false;

            hasAnyWeapon = hasAnyWeapon || primary != null;
            hasAnyWeapon = hasAnyWeapon || secondary != null;
            hasAnyWeapon = hasAnyWeapon || grenade != null;

            return hasAnyWeapon;
        }
    }

    public void AddWeapon (Transform weapon) {
        weapon.SetParent(weaponHolder);
        weapon.localPosition = Vector3.zero;
        weapon.localRotation = Quaternion.identity;

        CheckCurrentWeapons();

        DisableWeapons();

        if (HasAnyWeapons) {
            CycleWeapon(1); // Set weapon to first available.
        }
    }

    // Gets called when the player changes weapons by scrolling.
    public void CycleWeapon(int delta) {
        // Don't cycle if we don't have any weapons.
        if (!HasAnyWeapons) return;

        if (delta == 0) return; // Realistically this would never happen. 

        CycleWeaponIndex(delta);

        // Keep cycling while the current slot if available.
        while (IsSlotAvailable(index)) CycleWeaponIndex(delta);

        SetWeapon();
    }

    // Cycles the weapon index between 0 and 2
    private void CycleWeaponIndex(int delta) {
        index += delta;

        if (index > 1) {
            index = 0;
        }
        else if (index < 0) {
            index = 1;
        }
    }

    // Returns whether the specified slot is available.
    public bool IsSlotAvailable(int slot) {
        switch (slot) {
            case 0:
                return primary == null;
            case 1:
                return secondary == null;
            case 2:
                return grenade == null;
            default:
                Debug.LogError($"Invalid slot number '{slot}'!");
                return false;
        }
    }

    public void SetWeapon() {
        if (!HasAnyWeapons) return;

        DisableWeapons();

        CurrentWeapon.SetActive(true);
        CurrentWeapon.GetComponent<WeaponSystem>().Enable();
    }

    void Start () {
        CheckCurrentWeapons();

        DisableWeapons();

        index = -1;

        if (HasAnyWeapons) {
            CycleWeapon(1); // Set weapon to first available.
        }
    }

    void Update() {
        if (InputManager.GetAxis("Mouse ScrollWheel") > 0) {
            CycleWeapon(1);
        }
        else if (InputManager.GetAxis("Mouse ScrollWheel") < 0) {
            CycleWeapon(-1);
        }
    }

    private GameObject GetCurrentWeapon () {
        return index == 0 ? primary : secondary;
    }

    public void DisableWeapons () {
        if (primary) primary.SetActive(false);
        if (secondary) secondary.SetActive(false);
        if (grenade) grenade.SetActive(false);
    }

    public void CheckCurrentWeapons () {
        if (weaponHolder.childCount >= 1) primary = weaponHolder.GetChild(0)?.gameObject;
        if (weaponHolder.childCount >= 2) secondary = weaponHolder.GetChild(1)?.gameObject;
        if (weaponHolder.childCount >= 3) grenade = weaponHolder.GetChild(2)?.gameObject;
    }
}
