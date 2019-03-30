using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {
    public WeaponMovementProfile weaponMovementProfile;
    public GameObject worldModel;
    public WeaponMovement weaponMovement;

    public bool isFiringPrimary;
    public bool isFiringSecondary;

    public void Enable () {
        weaponMovement.profile = weaponMovementProfile;
    }

    public void Disable() {
        isFiringPrimary = isFiringSecondary = false;
    }

    void Update() {
        if (InputManager.GetButtonDown("Fire1") && !isFiringPrimary) {
            OnStartPrimary();

            isFiringPrimary = true;
        } else if (InputManager.GetButtonUp("Fire1") && isFiringPrimary) {
            OnEndPrimary();

            isFiringPrimary = false;
        }

        if (InputManager.GetButtonDown("Fire2") && !isFiringSecondary) {
            OnStartSecondary();

            isFiringSecondary = true;
        }
        else if (InputManager.GetButtonUp("Fire2") && isFiringSecondary) {
            OnEndSecondary();

            isFiringSecondary = false;
        }

        if (InputManager.GetButtonDown("Reload")) {
            OnReload();
        }
    }

    public virtual void OnReload () { }

    public virtual void OnStartPrimary() { }
    public virtual void OnEndPrimary() { }

    public virtual void OnStartSecondary() { }
    public virtual void OnEndSecondary() { }
}
