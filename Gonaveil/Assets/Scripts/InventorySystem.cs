using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public WeaponParameters[] allWeapons;
    public Weapon weaponMaster;
    private GameObject currentDropObject;
    public int selectedWeaponID;

    private WeaponMovement weaponMovement;
    private int lastSelectedWeaponID = -1;
    private WeaponParameters current;
    private bool disabled;

    void Start()
    {
        weaponMovement = GetComponent<WeaponMovement>();
    }

    void Update()
    {
        CheckInventory();
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            ++selectedWeaponID;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            --selectedWeaponID;
        }
        if (selectedWeaponID > 1)
        {
            selectedWeaponID = 0;
        } else if (selectedWeaponID < 0)
        {
            selectedWeaponID = 1;
        }
        if (allWeapons[selectedWeaponID] == null)
        {
            if (selectedWeaponID == 1)
            {
                selectedWeaponID = 0;
            }
            else if (selectedWeaponID == 0)
            {
                selectedWeaponID = 1;
            }
        }
        if (lastSelectedWeaponID != selectedWeaponID && !disabled)
        {
            SetWeapon();
        }
        if (Input.GetButtonDown("Drop Weapon"))
        {
            DropWeapon();
        }
    }

    void CheckInventory()
    {
        if(allWeapons[0] == null && allWeapons[1] == null)
        {
            weaponMaster.Disarm();
            disabled = true;
        }
        else
        {
            weaponMaster.Rearm();
            disabled = false;
        }
    }

    void DropWeapon()
    {
        allWeapons[selectedWeaponID] = null;
        Instantiate(currentDropObject, transform.position + transform.forward * 10, transform.rotation);
    }

    void SetWeapon() {
        //if (lastSelectedWeaponID > -1) allWeapons[lastSelectedWeaponID].weaponStats.modelObject.SetActive(false);

        current = allWeapons[selectedWeaponID];

        weaponMaster.SetParameters(current);
        weaponMovement.Profile = current.weaponMovementProfile;
        weaponMovement.offset = current.offset;

        currentDropObject = current.weaponDropPrefab;
        lastSelectedWeaponID = selectedWeaponID;
    }
}
