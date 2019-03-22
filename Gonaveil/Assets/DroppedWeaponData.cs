using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeaponData : MonoBehaviour
{
    public WeaponParameters weaponParameters;
    private CapsuleCollider pickupTrigger;

    public void Intangible(CapsuleCollider playerCollider)
    {
        pickupTrigger = playerCollider;
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), pickupTrigger, true);
        Invoke("Tangible", 1);
    }

    void Tangible()
    {
        Physics.IgnoreCollision(GetComponent<BoxCollider>(), pickupTrigger, false); 
    }
}
