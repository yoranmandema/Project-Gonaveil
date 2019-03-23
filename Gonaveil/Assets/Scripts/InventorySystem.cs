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
    private int lastSelectedWeaponID = -1;
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

    void Start() {
        //weaponMovement = GetComponent<WeaponMovement>();
        lastSelectedWeaponID = 1;
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

    private void IncrementWeaponIndex() {
        if (++selectedWeaponID > 2) selectedWeaponID = 0;
    }

    private void DecrementWeaponIndex() {
        if (--selectedWeaponID < 0) selectedWeaponID = 2;
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
        if (delta == 0) return; // Realistically this would never happen. 

        CycleWeaponIndex(delta);

        while (isSlotAvailable(selectedWeaponID)) CycleWeaponIndex(delta);

        SetWeapon();

        print(selectedWeaponID);
    }

    private void Cycle () {
        if (!HasAnyWeapons) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            CycleWeapon(1);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            CycleWeapon(-1);
        }
    } 

    void Update() {
        CheckInventory();

        Cycle();

        if (lastSelectedWeaponID != selectedWeaponID) {
            SetWeapon();
        }

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
        GameObject dropItem = Instantiate(currentDropObject, transform.position + transform.up, transform.rotation) as GameObject;
        dropItem.GetComponent<DroppedWeaponData>().Intangible(GetComponentInChildren<CapsuleCollider>());
        dropItem.GetComponent<DroppedWeaponData>().weaponParameters = CurrentWeapon;
        dropItem.transform.Rotate(0, 90, 0);

        CurrentWeapon = null;

        var throwVector = transform.forward + Vector3.up;
        dropItem.GetComponent<Rigidbody>().AddForce(throwVector * 100);
    }

    private void OnTriggerStay(Collider collision) {
        if (collision.tag == "ItemWeapon") {
            WeaponParameters droppedParameters = collision.GetComponentInChildren<DroppedWeaponData>().weaponParameters;
            //CheckInventory(0, 1, collision.gameObject, droppedParameters, currentWeaponText);
            //CheckInventory(1, 0, collision.gameObject, droppedParameters, holsteredWeaponText);


        }
    }

    void TryToPickupWeapon(GameObject item, WeaponParameters droppedParameters) {
        //if (allWeapons[PrimaryID] == null) {
        //    try {
        //        if (allWeapons[SecondaryID].name != droppedParameters.name) {
        //            PickupItem(PrimaryID, SecondaryID, item, droppedParameters, TextDisplay);
        //        }
        //    }
        //    catch {
        //        PickupItem(PrimaryID, SecondaryID, item, droppedParameters, TextDisplay);
        //    }
        //}

        //if (allWeapons[0] == null) {

        //}

        //PickupItem(PrimaryID, SecondaryID, item, droppedParameters, TextDisplay);
    }

    void PickupItem(int InventorySlot, int SecondarySlot, GameObject item, WeaponParameters droppedParameters) {
        CurrentWeapon = droppedParameters;

        lastSelectedWeaponID = SecondarySlot;

        Destroy(item);
    }

    void SetWeapon() {
        if (!HasAnyWeapons) return;

        current = CurrentWeapon;

        weaponMaster.SetParameters(current);
        weaponMovement.Profile = current.weaponMovementProfile;
        weaponMovement.offset = current.offset;

        currentDropObject = current.weaponDropPrefab;

        lastSelectedWeaponID = selectedWeaponID;
    }
}
