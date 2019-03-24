using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponAsset", menuName = "Weapons/New Weapon Asset")]
public class WeaponAsset : ScriptableObject {
    [SerializeField] public string primaryComponentName;
    [SerializeField] public string secondaryComponentName;

    public WeaponComponentProfile primaryProfile;
    public WeaponComponentProfile secondaryProfile;

    public WeaponMovementProfile weaponMovementProfile;
    public GameObject viewModel;
    public GameObject worldModel;
}
