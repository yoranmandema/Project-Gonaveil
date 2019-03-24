using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewWeaponParameters", menuName = "Weapons/New Weapon Parameters")]
public class WeaponParameters : ScriptableObject
{
    public string weaponName;
    public WeaponValues weaponStats;
    public WeaponMovementProfile weaponMovementProfile;
    public GameObject viewModel;
    public GameObject weaponDropPrefab;
    public bool isGrenade;
}
