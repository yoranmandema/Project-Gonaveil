using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour {
    [SerializeField]
    private Weapon currentWeapon;

    private void Start() {
        currentWeapon = GetComponentInChildren<Weapon>();
    }
}
