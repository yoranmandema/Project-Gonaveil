using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class WeaponComponent : MonoBehaviour {
    [HideInInspector] public WeaponController weaponController;
    [HideInInspector] public Transform camera;

    public WeaponComponentProfile Profile { get; set; }

    public virtual void OnFireStart() { }
    public virtual void OnFireEnd() { }

    public void Initialise(Transform cam, WeaponController weapon, WeaponComponentProfile _profile) {
        camera = cam;
        weaponController = weapon;
        Profile = _profile;
    }


}