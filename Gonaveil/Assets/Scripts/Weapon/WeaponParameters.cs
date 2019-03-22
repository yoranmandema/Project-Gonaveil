using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWeaponParameters", menuName = "Weapons/New Weapon Parameters")]
public class WeaponParameters : ScriptableObject
{
    public WeaponValues weaponStats;
    public WeaponMovementProfile weaponMovementProfile;
    public GameObject viewModel;
    public Vector3 offset = new Vector3(0.35f, -0.3f, 0.6f);
}
