using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour {
    public GameObject PlayerObject;

    [SerializeField]
    private Weapon currentWeapon;
    private Animator anim;

    private void Start() {
        currentWeapon = GetComponentInChildren<Weapon>();
        anim = PlayerObject.GetComponentInChildren<Animator>();
    }

    private void FixedUpdate() {
        anim.SetInteger("WeaponType", (int)currentWeapon.weaponValues.weaponType);
    }
}
