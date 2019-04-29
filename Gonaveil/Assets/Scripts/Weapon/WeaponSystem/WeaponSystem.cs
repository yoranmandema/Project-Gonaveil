using System.Collections;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {
    [HideInInspector] public bool isFiringPrimary;
    [HideInInspector] public bool isFiringSecondary;
    [HideInInspector] public bool isReloading;

    public string weaponName;

    public int ammo;
    public int clipSize;

    public float fireRate = 800;
    public int burst = 0;
    public bool forceBurst = false;

    public bool allowQueueFire = true;

    public bool autoReload = true;
    public float reloadTime;

    public bool queueFiring = false;

    public bool hasSecondary = true;

    public AnimationCurve accuracyCurve;
    public GameObject projectile;
    public LayerMask lineCastMask;
    public WeaponMovementProfile weaponMovementProfile;
    public GameObject worldModel;
    public WeaponMovement weaponMovement;
    public new Transform camera;
    public bool isGrenade;

    protected float lastFireTime;
    protected WeaponModelData weaponModelData;
    protected float accuracyTime;
    protected float accuracy;
    protected int burstNum;
    protected bool isBursting;
    protected bool queuedFire;

    public void Enable() {
        camera = transform.root.GetComponentInChildren<Camera>().transform;

        if (camera == null) {
            Debug.LogError("No camera found on weapon holder!");
        }

        weaponMovement = transform.parent.GetComponent<WeaponMovement>();
        weaponMovement.profile = weaponMovementProfile;

        OnEnable();
    }

    public void Disable() {
        isFiringPrimary = isFiringSecondary = false;

        OnDisable();
    }

    private void Start() {
        weaponModelData = GetComponent<WeaponModelData>();

        OnStart();
    }

    private void Update() {

        if (accuracyCurve.length > 0) {
            var accuracyCurveEnd = accuracyCurve[accuracyCurve.length - 1].time;

            accuracyTime = Mathf.Clamp(accuracyTime + Time.deltaTime * (isFiringPrimary ? 1 : -1), 0, accuracyCurveEnd);

            accuracy = accuracyCurve.Evaluate(accuracyTime);
        }

        if (InputManager.GetButtonDown("Fire1") && !isFiringPrimary && !isReloading) {
            OnStartPrimary();

            Fire();

            isFiringPrimary = true;

            if (allowQueueFire) queuedFire = true;

            isBursting = true;
        }
        else if (InputManager.GetButtonUp("Fire1") && isFiringPrimary) {
            OnEndPrimary();

            StopFire();
        }

        if (InputManager.GetButtonDown("Fire2") && !isFiringSecondary && hasSecondary) {
            OnStartSecondary();

            isFiringSecondary = true;
        }
        else if (InputManager.GetButtonUp("Fire2") && isFiringSecondary) {
            OnEndSecondary();

            isFiringSecondary = false;
        }

        if (InputManager.GetButtonDown("Reload")) {
            OnStartReload();
        }

        if ((burstNum < burst || burst == 0) && (isFiringPrimary || (isBursting && forceBurst))) {
            Fire();
        }

        OnUpdate();
    }

    private void StopFire () {
        isFiringPrimary = false;

        burstNum = 0;

        if (!forceBurst) isBursting = false;
    }

    public virtual void OnEnable() { }
    public virtual void OnDisable() { }

    public virtual void OnStart() {
        Disable();

        ammo = clipSize;
    }
    public virtual void OnUpdate() { }

    public virtual void OnStartReload() {
        StopFire();

        StartCoroutine(ReloadCoroutine());
    }

    protected void ConsumeAmmo() {
        if (ammo > 0) {
            ammo -= 1;
        }
        
        if (autoReload && ammo == 0) {
            OnStartReload();
        }
    }

    protected void ConsumeFireSample() {
        lastFireTime = Time.realtimeSinceStartup;
    }

    protected bool CheckFireTime () {
        return (Time.realtimeSinceStartup - lastFireTime) > (1 / (fireRate / 60));
    }

    private void Fire () {
        if (ammo == 0) return;
        if (!CheckFireTime()) return;

        queuedFire = false;

        burstNum++;

        if (burstNum == burst - 1) { 
            isBursting = false;
        }

        ConsumeFireSample();
        ConsumeAmmo();

        OnFire();
    }

    public virtual void OnEndReload() {
        ammo = clipSize;
    }

    public virtual void OnStartPrimary() { }
    public virtual void OnEndPrimary() { }

    public virtual void OnStartSecondary() { }
    public virtual void OnEndSecondary() { }

    public virtual void OnFire() { }

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

    protected void FireProjectile(float spread = 0) {
        var projectileObject = Instantiate(projectile, camera.position, camera.rotation) as GameObject;

        var spreadVector = CalculateSpreadVector(spread);

        var projectileComponent = projectileObject.GetComponent<Projectile>();

        projectileObject.transform.rotation = Quaternion.LookRotation(spreadVector);
        projectileComponent.barrel = weaponModelData.barrel;
        projectileComponent.instigator = transform.root.gameObject;
        projectileComponent.weaponSystem = this;
        projectileComponent.Fire();
    }

    protected bool FireLine(out RaycastHit hitResult, float spread = 0, float maxRange = Mathf.Infinity) {
        var spreadVector = CalculateSpreadVector(spread);

        return Physics.Raycast(camera.position, spreadVector, out hitResult, maxRange, lineCastMask);
    }

    protected virtual IEnumerator ReloadCoroutine() {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        isReloading = false;

        OnEndReload();
    }
}
