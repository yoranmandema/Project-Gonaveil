using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {
    public WeaponMovementProfile weaponMovementProfile;
    public GameObject worldModel;
    public WeaponMovement weaponMovement;
    public new Transform camera;
    public bool isGrenade;

    protected WeaponModelData weaponModelData;
    [HideInInspector] public bool isFiringPrimary;
    [HideInInspector] public bool isFiringSecondary;

    public void Enable () {
        weaponMovement.profile = weaponMovementProfile;
    }

    public void Disable() {
        isFiringPrimary = isFiringSecondary = false;
    }

    void Start () {
        weaponModelData = GetComponent<WeaponModelData>();

        OnStart();
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

        OnUpdate();
    }

    public virtual void OnStart() { }
    public virtual void OnUpdate () { }

    public virtual void OnReload () { }

    public virtual void OnStartPrimary() { }
    public virtual void OnEndPrimary() { }

    public virtual void OnStartSecondary() { }
    public virtual void OnEndSecondary() { }

    // Utility functions

    protected Vector3 CalculateSpreadVector(float max) {
        var spreadVector = Vector3.zero;

        var angle = Random.Range(0, 2 * Mathf.PI);
        var offset = Random.Range(0, max * Mathf.Deg2Rad);

        spreadVector += camera.right * Mathf.Cos(angle) * Mathf.Sin(offset);
        spreadVector += camera.up * Mathf.Sin(angle) * Mathf.Sin(offset);
        spreadVector += camera.forward * Mathf.Cos(offset);

        return spreadVector.normalized;
    }
}
