using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public int primaryWeaponID;
    public int secondaryWeaponID;
    public WeaponParameters[] allWeapons;
    public Weapon weaponMaster;
    public int selectedWeaponID;

    private WeaponMovement weaponMovement;
    private int lastSelectedWeaponID = -1;

    void Start()
    {
        //allWeapons = FindObjectsOfType<WeaponParameters>();

        weaponMovement = GetComponent<WeaponMovement>();

        SetWeapon();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            if (++selectedWeaponID > 1) selectedWeaponID = 0;

            SetWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            if (--selectedWeaponID < 0) selectedWeaponID = 1;

            SetWeapon();
        }
    }

    void SetWeapon() {
        //if (lastSelectedWeaponID > -1) allWeapons[lastSelectedWeaponID].weaponStats.modelObject.SetActive(false);

        var current = allWeapons[selectedWeaponID];

        weaponMaster.SetParameters(current);
        weaponMovement.offset = current.offset;
        weaponMovement.Profile = current.weaponMovementProfile;

        lastSelectedWeaponID = selectedWeaponID;
    }
}
