using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponComponent : MonoBehaviour {
    public WeaponController weaponController;
    public Transform camera;

    public abstract void OnFireStart();
    public abstract void OnFireEnd();
}
