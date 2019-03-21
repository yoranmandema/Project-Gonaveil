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
    // Start is called before the first frame update
    void Start()
    {
        allWeapons = FindObjectsOfType<WeaponParameters>();
        allWeapons[primaryWeaponID].weaponStats.modelObject.SetActive(true);
        weaponMaster.weaponValues = allWeapons[primaryWeaponID].weaponStats;
    }

    // Update is called once per frame
    void Update()
    {
        if(selectedWeaponID == 1)
        {
            allWeapons[secondaryWeaponID].weaponStats.modelObject.SetActive(false);
            allWeapons[primaryWeaponID].weaponStats.modelObject.SetActive(true);
            weaponMaster.weaponValues = allWeapons[primaryWeaponID].weaponStats;
        }else if(selectedWeaponID == 2)
        {
            allWeapons[primaryWeaponID].weaponStats.modelObject.SetActive(false);
            allWeapons[secondaryWeaponID].weaponStats.modelObject.SetActive(true);
            weaponMaster.weaponValues = allWeapons[secondaryWeaponID].weaponStats;
        }
        selectedWeaponID += (int)Input.GetAxis("Mouse ScrollWheel");
        if (selectedWeaponID > 2)
        {
            selectedWeaponID = 1;
        }
        else if (selectedWeaponID < 1)
        {
            selectedWeaponID = 2;
        }
    }
}
