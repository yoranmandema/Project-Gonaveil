using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public WeaponParameters[] allWeapons;
    public Weapon weaponMaster;
    private GameObject currentDropObject;
    public int selectedWeaponID;
    public WeaponMovement weaponMovement;

    public UnityEngine.UI.Text currentWeaponText;
    public UnityEngine.UI.Text holsteredWeaponText;

    private int lastSelectedWeaponID = -1;
    private WeaponParameters current;
    private bool disabled;

    void Start()
    {
        //weaponMovement = GetComponent<WeaponMovement>();
        lastSelectedWeaponID = 1;
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
        if (Input.GetButtonDown("Drop Weapon") && !disabled)
        {
            DropWeapon();
        }
    }

    void CheckInventory()
    {
        if(allWeapons[0] == null && allWeapons[1] == null)
        {
            currentWeaponText.text = "None";
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
        holsteredWeaponText.text = "None";
        GameObject dropItem = Instantiate(currentDropObject, transform.position + transform.up, transform.rotation) as GameObject;
        dropItem.GetComponent<DroppedWeaponData>().Intangible(GetComponentInChildren<CapsuleCollider>());
        dropItem.GetComponent<DroppedWeaponData>().weaponParameters = allWeapons[selectedWeaponID];
        allWeapons[selectedWeaponID] = null;
        dropItem.transform.Rotate(0, 90, 0);
        var throwVector = new Vector3();
        throwVector = transform.forward + Vector3.up;
        dropItem.GetComponent<Rigidbody>().AddForce(throwVector * 100);
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "ItemWeapon")
        {
            WeaponParameters droppedParameters = collision.GetComponentInChildren<DroppedWeaponData>().weaponParameters;
            CheckInventory(0, 1, collision.gameObject, droppedParameters, currentWeaponText);
            CheckInventory(1, 0, collision.gameObject, droppedParameters, holsteredWeaponText);
        }
    }

    void CheckInventory(int PrimaryID, int SecondaryID, GameObject item, WeaponParameters droppedParameters, UnityEngine.UI.Text TextDisplay)
    {
        if (allWeapons[PrimaryID] == null)
        {
            try
            {
                if (allWeapons[SecondaryID].name != droppedParameters.name)
                {
                    PickupItem(PrimaryID, SecondaryID, item, droppedParameters, TextDisplay);
                }
            }
            catch
            {
                PickupItem(PrimaryID, SecondaryID, item, droppedParameters, TextDisplay);
            }
        }
    }

    void PickupItem(int InventorySlot, int SecondarySlot, GameObject item, WeaponParameters droppedParameters, UnityEngine.UI.Text TextDisplay)
    {
        allWeapons[InventorySlot] = droppedParameters;
        TextDisplay.text = allWeapons[InventorySlot].name;
        lastSelectedWeaponID = SecondarySlot;
        Destroy(item);
    }

    void SetWeapon() {
        //if (lastSelectedWeaponID > -1) allWeapons[lastSelectedWeaponID].weaponStats.modelObject.SetActive(false);
        currentWeaponText.text = allWeapons[selectedWeaponID].name;
        current = allWeapons[selectedWeaponID];

        weaponMaster.SetParameters(current);
        weaponMovement.Profile = current.weaponMovementProfile;
        weaponMovement.offset = current.offset;

        currentDropObject = current.weaponDropPrefab;
        if (lastSelectedWeaponID != -1 && allWeapons[lastSelectedWeaponID] != null)
        {
            holsteredWeaponText.text = allWeapons[lastSelectedWeaponID].name;
        }
        lastSelectedWeaponID = selectedWeaponID;
    }
}
