using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeaponData : MonoBehaviour
{
    public WeaponParameters weaponParameters;
    public int currentMagazineCapacity;
    public int currentAmmoPool;
    private CapsuleCollider pickupTrigger;

    //apply the ammo in the inventory to the dropped weapon.
    public void SetDroppedAmmo(int magazine, int ammoPool)
    {
        currentMagazineCapacity = magazine;
        currentAmmoPool = ammoPool;
    }
    
    //used to enusre the weapon cannot be picked up for a short time.
    public void Intangible(CapsuleCollider playerCollider)
    {
        //get the collider and ignore collision
        pickupTrigger = playerCollider;
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), pickupTrigger, true);
        //invoke another class to allow the weapon to be picked up again
        Invoke("Tangible", 0.5f);
    }

    void Tangible()
    {
        //stop ignoring
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), pickupTrigger, false); 
    }
}
