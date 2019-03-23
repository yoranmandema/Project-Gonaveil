using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour {
    public WeaponParameters primary;
    public WeaponParameters secondary;
    public WeaponParameters grenade;

    public Weapon weaponMaster;
    public int selectedWeaponID;
    public WeaponMovement weaponMovement;

    private GameObject currentDropObject;
    private WeaponParameters current;

    private WeaponParameters CurrentWeapon { get {
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

        set {
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

    private bool HasAnyWeapons {
        get {
            var hasAnyWeapon = false;

            hasAnyWeapon = hasAnyWeapon || primary != null;
            hasAnyWeapon = hasAnyWeapon || secondary != null;
            hasAnyWeapon = hasAnyWeapon || grenade != null;

            return hasAnyWeapon;
        }
    }

    private bool isSlotAvailable (int slot) {
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

        SetWeapon();
    }

    private void Cycle () {
        // Don't cycle if we don't have any weapons.
        if (!HasAnyWeapons) return;

        if (InputManager.GetAxis("Mouse ScrollWheel") > 0) {
            CycleWeapon(1);
        }
        else if (InputManager.GetAxis("Mouse ScrollWheel") < 0) {
            CycleWeapon(-1);
        } else if (InputManager.GetButtonDown("Grenade")) {

            // Only switch if we actually have a grenade.
            if (grenade != null) {
                selectedWeaponID = 2;

                SetWeapon();
            }
        }
    } 

    private void Start () {
        selectedWeaponID = -1;

        CycleWeapon(1); // Set weapon to first available.
    }

    private void Update() {
        Cycle();

        if (InputManager.GetButtonDown("Drop Weapon")) {
            DropWeapon();
        }
    }

    void CheckInventory() {
        if (!HasAnyWeapons) {
            weaponMaster.Disarm();
        }
        else {
            weaponMaster.Rearm();
        }
    }

    void DropWeapon() {
        var dropItem = Instantiate(currentDropObject, transform.position + transform.up, transform.rotation) as GameObject;
        dropItem.transform.Rotate(0, 90, 0);

        var dropData = dropItem.GetComponent<DroppedWeaponData>();
        dropData.Intangible(GetComponentInChildren<CapsuleCollider>());
        dropData.weaponParameters = CurrentWeapon;

        var throwVector = transform.forward + Vector3.up;
        dropItem.GetComponent<Rigidbody>().AddForce(throwVector * 200);

        CurrentWeapon = null;

        CycleWeapon(1); // Set weapon to first available.

        CheckInventory();
    }

    private void OnTriggerStay(Collider collision) {
        if (collision.tag == "ItemWeapon") {
            var dropData = collision.GetComponentInChildren<DroppedWeaponData>();

            if (dropData == null) return;

            var droppedParameters = dropData.weaponParameters;

            PickupWeapon(collision.gameObject, droppedParameters);
        }
    }

    void PickupWeapon(GameObject item, WeaponParameters droppedParameters) {

        if (!droppedParameters.isGrenade) {
            if (primary == null) {
                primary = droppedParameters;
            } else if (secondary == null) {
                secondary = droppedParameters;
            }
        } else {
            grenade = droppedParameters;
        }

        Destroy(item);
    }

    void SetWeapon() {
        if (!HasAnyWeapons) return;

        current = CurrentWeapon;

        weaponMaster.SetParameters(current);
        weaponMovement.Profile = current.weaponMovementProfile;
        weaponMovement.offset = current.offset;

        currentDropObject = current.weaponDropPrefab;
    }
}
