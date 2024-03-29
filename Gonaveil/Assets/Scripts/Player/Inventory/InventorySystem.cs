﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour {

    [System.Serializable]
    public class InventorySlot {
        public string weaponName;
        public int weaponMagazine;
        public int weaponAmmoPool;
        public WeaponParameters weaponParameters;

        public void SetInventoryAmmo(int magazine, int ammoPool) {
            weaponMagazine = magazine;
            weaponAmmoPool = ammoPool;
        }
    }

    public InventorySlot primary;
    public InventorySlot secondary;
    public InventorySlot grenade;

    public Weapon weaponMaster;
    public int selectedWeaponID;
    public WeaponMovement weaponMovement;

    private GameObject currentDropObject;
    private WeaponParameters current;

    public InventorySlot CurrentWeapon {
        get {
            switch (selectedWeaponID) {
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
            switch (selectedWeaponID) {
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

            hasAnyWeapon = hasAnyWeapon || primary.weaponParameters != null;
            hasAnyWeapon = hasAnyWeapon || secondary.weaponParameters != null;
            hasAnyWeapon = hasAnyWeapon || grenade.weaponParameters != null;

            return hasAnyWeapon;
        }
    }

    // Returns whether the specified slot is available.
    private bool isSlotAvailable(int slot) {
        switch (slot) {
            case 0:
                return primary.weaponParameters == null;
            case 1:
                return secondary.weaponParameters == null;
            case 2:
                return grenade.weaponParameters == null;
            default:
                Debug.LogError($"Invalid slot number '{slot}'!");
                return false;
        }
    }

    // Cycles the weapon index between 0 and 2
    private void CycleWeaponIndex(int delta) {
        selectedWeaponID += delta;

        if (selectedWeaponID > 2) {
            selectedWeaponID = 0;
        }
        else if (selectedWeaponID < 0) {
            selectedWeaponID = 2;
        }
    }

    // Gets called when the player changes weapons by scrolling.
    private void CycleWeapon(int delta) {
        // Don't cycle if we don't have any weapons.
        if (!HasAnyWeapons) return;

        if (delta == 0) return; // Realistically this would never happen. 

        CycleWeaponIndex(delta);

        // Keep cycling while the current slot if available.
        while (isSlotAvailable(selectedWeaponID)) CycleWeaponIndex(delta);

        SetWeapon(true);
    }

    private void Cycle() {
        // Don't cycle if we don't have any weapons.
        if (!HasAnyWeapons) return;

        if (InputManager.GetAxis("Mouse ScrollWheel") > 0) {
            CycleWeapon(1);
        }
        else if (InputManager.GetAxis("Mouse ScrollWheel") < 0) {
            CycleWeapon(-1);
        }
        else if (InputManager.GetButtonDown("Grenade")) {

            // Only switch if we actually have a grenade.
            if (grenade.weaponParameters != null) {
                selectedWeaponID = 2;

                SetWeapon(true);
            }
        }
    }

    private void Start() {
        selectedWeaponID = -1;

        if (!HasAnyWeapons) {
            weaponMaster.Disarm();
        }
        else {
            CycleWeapon(1); // Set weapon to first available.
            weaponMaster.Rearm();
        }
    }

    private void UpdateAmmo() {
        if (!HasAnyWeapons) return;
        CurrentWeapon.SetInventoryAmmo(weaponMaster.currentMagazine, weaponMaster.currentAmmoPool);
    }

    private void Update() {
        Cycle();
        UpdateAmmo();
        CheckGrenades();
        if (InputManager.GetButtonDown("Drop Weapon") && HasAnyWeapons) {
            DropWeapon();
        }
    }

    private void CheckGrenades()
    {
        if(grenade.weaponMagazine == 0 && grenade.weaponParameters != null)
        {
            grenade.weaponParameters = null;
            CycleWeapon(1);

            if (!HasAnyWeapons) weaponMaster.Disarm();
        }
    }

    void DropWeapon() {
        var dropItem = Instantiate(currentDropObject, transform.position + transform.up, transform.rotation) as GameObject;
        dropItem.transform.Rotate(0, 90, 0);

        var dropData = dropItem.GetComponentInChildren<DroppedWeaponData>();
        dropData.Intangible(GetComponentInChildren<CapsuleCollider>());
        dropData.weaponParameters = CurrentWeapon.weaponParameters;
        dropData.SetDroppedAmmo(CurrentWeapon.weaponMagazine, CurrentWeapon.weaponAmmoPool);

        var throwVector = transform.forward + Vector3.up;
        dropItem.GetComponent<Rigidbody>().AddForce(throwVector * 200);

        CurrentWeapon.weaponParameters = null;

        CycleWeapon(1); // Set weapon to first available.

        if (!HasAnyWeapons) weaponMaster.Disarm();
    }

    private void OnTriggerStay(Collider collision) {
        if (collision.tag == "ItemWeapon") {
            var dropData = collision.GetComponentInChildren<DroppedWeaponData>();

            if (dropData == null) return;

            PickupWeapon(collision.gameObject, dropData);
        }
    }

    void PickupWeapon(GameObject item, DroppedWeaponData dropData) {
        bool WeaponFull = false;
        bool GrenadeFull = false;
        if (!dropData.weaponParameters.isGrenade) {
            if (primary.weaponParameters == null && secondary.weaponName != dropData.weaponParameters.weaponName) {
                primary.weaponParameters = dropData.weaponParameters;
                primary.weaponName = dropData.weaponParameters.weaponName;
                primary.SetInventoryAmmo(dropData.currentMagazineCapacity, dropData.currentAmmoPool);
            }
            else if (secondary.weaponParameters == null && primary.weaponName != dropData.weaponParameters.weaponName) {
                secondary.weaponParameters = dropData.weaponParameters;
                secondary.weaponName = dropData.weaponParameters.weaponName;
                secondary.SetInventoryAmmo(dropData.currentMagazineCapacity, dropData.currentAmmoPool);
            }
            else {
                WeaponFull = true;
            }
        }
        else {
            if (grenade.weaponParameters == null) {
                grenade.weaponParameters = dropData.weaponParameters;
                grenade.SetInventoryAmmo(dropData.currentMagazineCapacity, dropData.currentAmmoPool);
            }
            else {
                GrenadeFull = true;
            }
        }
        if (GrenadeFull)
        {
            if (grenade.weaponParameters.name == dropData.weaponParameters.name)
            {
                grenade.SetInventoryAmmo(grenade.weaponMagazine + dropData.currentMagazineCapacity + dropData.currentAmmoPool, 0);
                SetWeapon(false);
            }
            else
            {
                return;
            }
        }
        if (WeaponFull)
        {
            if(primary.weaponName == dropData.weaponParameters.weaponName)
            {
                primary.SetInventoryAmmo(primary.weaponMagazine, primary.weaponAmmoPool + dropData.currentAmmoPool + dropData.currentMagazineCapacity);
                SetWeapon(false);
            }else if(secondary.weaponName == dropData.weaponParameters.weaponName)
            {
                secondary.SetInventoryAmmo(secondary.weaponMagazine, secondary.weaponAmmoPool + dropData.currentAmmoPool + dropData.currentMagazineCapacity);
                SetWeapon(false);
            }
            else
            {
                return;
            }
        }
        else {
            if (HasAnyWeapons && !weaponMaster.weaponEquipped) {
                CycleWeapon(1); // Set weapon to first available.
                weaponMaster.Rearm();
            }
        }
        Destroy(item);

    }

    void SetWeapon(bool stopWeaponFire) {
        if (!HasAnyWeapons) return;

        current = CurrentWeapon.weaponParameters;

        weaponMaster.SetParameters(current, stopWeaponFire);
        weaponMaster.SetWeaponAmmo(CurrentWeapon.weaponMagazine, CurrentWeapon.weaponAmmoPool);
        weaponMovement.profile = current.weaponMovementProfile;

        currentDropObject = current.weaponDropPrefab;
    }
}
