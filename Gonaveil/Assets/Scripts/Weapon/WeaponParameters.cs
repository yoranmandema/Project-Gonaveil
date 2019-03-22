using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponParameters : MonoBehaviour
{
    public Weapon.WeaponValues weaponStats;
    public WeaponMovementProfile weaponMovementProfile;
    public Vector3 offset = new Vector3(0.35f, -0.3f, 0.6f);
}
