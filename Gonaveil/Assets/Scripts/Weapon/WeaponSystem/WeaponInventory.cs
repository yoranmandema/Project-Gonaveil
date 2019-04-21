using UnityEngine;

public class WeaponInventory : MonoBehaviour {
    public GameObject primary;
    public GameObject secondary;
    public GameObject grenade;
    public bool autoSwitch = true;
    public Transform weaponHolder;
    public GameObject droppedWeaponPrefab;

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

    public void RemoveWeapon(WeaponSystem weapon) {
        var slot = GetWeaponSlot(weapon);
        var formerIndex = index;

        if (slot > -1) {
            index = slot;

            CurrentWeapon.transform.SetParent(null);
            CurrentWeapon = null;

            index = formerIndex;

            if (slot == index) {
                if (HasAnyWeapons) {
                    CycleWeapon(1); // Set weapon to first available.
                }
            }

            ActivateWeapon(CurrentWeapon);
        }
    }

    private int GetWeaponSlot(WeaponSystem weapon) {
        var slot = -1;
        var weaponGameObject = weapon.gameObject;

        if (primary == weaponGameObject) {
            slot = 0;
        }
        else if (secondary == weaponGameObject) {
            slot = 1;
        }
        else if (grenade == weaponGameObject) {
            slot = 2;
        }

        return slot;
    }

    private int GetFirstAvailableSlot(WeaponSystem weapon) {
        var slot = -1;

        if (weapon.isGrenade && grenade == null) {
            slot = 2;
        }
        else {
            if (primary == null) {
                slot = 0;
            }
            else if (secondary == null) {
                slot = 1;
            }
        }

        return slot;
    }

    public void AddWeapon(WeaponSystem weapon) {
        var weaponTransform = weapon.transform;

        weaponTransform.SetParent(weaponHolder);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        var slot = GetFirstAvailableSlot(weapon);

        if (slot > -1) {
            DisableWeapon(weapon.gameObject);
            DisableWeapon(CurrentWeapon);

            if (autoSwitch) {
                index = slot;
            }

            CurrentWeapon = weapon.gameObject;

            ActivateWeapon(CurrentWeapon);
        }
    }

    private void ActivateWeapon(GameObject weapon) {
        if (weapon == null) return;

        weapon.SetActive(true);
        weapon.GetComponent<WeaponSystem>().Enable();
    }
    private void DisableWeapon(GameObject weapon) {
        if (weapon == null) return;

        weapon.SetActive(false);
        weapon.GetComponent<WeaponSystem>().Disable();
    }

    // Gets called when the player changes weapons by scrolling.
    public void CycleWeapon(int delta) {
        // Don't cycle if we don't have any weapons.
        if (!HasAnyWeapons) {
            return;
        }

        if (delta == 0) {
            return; // Realistically this would never happen. 
        }

        if (CurrentWeapon != null) {
            DisableWeapon(CurrentWeapon);
        }

        CycleWeaponIndex(delta);

        // Keep cycling while the current slot if available.
        while (IsSlotAvailable(index)) {
            CycleWeaponIndex(delta);
        }

        ActivateWeapon(CurrentWeapon);
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

    private void Start() {
        CheckCurrentWeapons();

        DisableWeapons();

        index = -1;

        if (HasAnyWeapons) {
            CycleWeapon(1); // Set weapon to first available.
        }
    }

    private void Update() {
        if (InputManager.GetAxis("Mouse ScrollWheel") > 0) {
            CycleWeapon(1);
        }
        else if (InputManager.GetAxis("Mouse ScrollWheel") < 0) {
            CycleWeapon(-1);
        }
        else if (InputManager.GetButtonDown("Drop Weapon")) {
            DropWeapon();
        }
    }

    private void DropWeapon() {
        if (CurrentWeapon == null) {
            return;
        }

        var weaponSystem = CurrentWeapon.GetComponent<WeaponSystem>();

        var cam = weaponSystem.camera;

        var drop = Instantiate(droppedWeaponPrefab, cam.position, Quaternion.identity);

        drop.transform.rotation = Quaternion.LookRotation(cam.forward.SetY(0));
        drop.GetComponent<Rigidbody>().velocity += GetComponent<PlayerMovement>().velocity / 3f + cam.forward * 7f;
        drop.GetComponent<DroppedWeapon>().SetWeapon(CurrentWeapon);
        drop.GetComponent<DroppedWeapon>().Initiate(gameObject);

        RemoveWeapon(weaponSystem);
    }

    private GameObject GetCurrentWeapon() {
        return index == 0 ? primary : secondary;
    }

    public void DisableWeapons() {
        if (primary) {
            DisableWeapon(primary);
        }

        if (secondary) {
            DisableWeapon(secondary);
        }

        if (grenade) {
            DisableWeapon(grenade);
        }
    }

    public void CheckCurrentWeapons() {
        if (weaponHolder.childCount >= 1) {
            primary = weaponHolder.GetChild(0)?.gameObject;
        }

        if (weaponHolder.childCount >= 2) {
            secondary = weaponHolder.GetChild(1)?.gameObject;
        }

        if (weaponHolder.childCount >= 3) {
            grenade = weaponHolder.GetChild(2)?.gameObject;
        }
    }
}
