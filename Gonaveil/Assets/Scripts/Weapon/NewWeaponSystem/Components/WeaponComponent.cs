using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponComponent : MonoBehaviour {
    [HideInInspector] public WeaponController weaponController;
    [HideInInspector] public Transform camera;

    public virtual void OnFireStart() { }
    public virtual void OnFireEnd() { }

    public void Initialise(Transform cam, WeaponController weapon) {
        camera = cam;
        weaponController = weapon;
    }
}