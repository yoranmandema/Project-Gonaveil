using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponAsset", menuName = "Weapons/New Weapon Asset")]
public class WeaponAsset : ScriptableObject {
    public System.Type primaryComponent;
    public System.Type secondaryComponent;

    public WeaponMovementProfile weaponMovementProfile;
    public GameObject viewModel;
    public GameObject worldModel;

    public Vector3 offset = new Vector3(0.35f, -0.3f, 0.6f);
}
