using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeaponData : MonoBehaviour
{
    public WeaponParameters weaponParameters;

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("TOUCH");
        if (collision.transform.tag == "Player")
        {
            InventorySystem playerInvo = collision.transform.GetComponentInChildren<InventorySystem>();
            if (playerInvo.allWeapons[0] == null)
            {
                Destroy(gameObject);
                playerInvo.allWeapons[0] = weaponParameters;
            }
            else if (playerInvo.allWeapons[1] == null)
            {
                Destroy(gameObject);
                playerInvo.allWeapons[1] = weaponParameters;
            }
        }
    }
}
