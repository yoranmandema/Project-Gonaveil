using UnityEngine;

public class Weapon : MonoBehaviour {
    public UnityEngine.UI.Image chargeCircle;
    public WeaponParameters weaponParameters;
    public LayerMask raycastMask;
    public GameObject impact;
    public Animator animator;

    public enum FireStage { Idle, Firing, Charging }
    [HideInInspector] public float chargeProgress;
    public GameObject viewModel;

    private Camera mainCamera;
    private float loadTimer;
    private float burstTimer;
    private float burstCount;
    private FireStage fireStage;
    private Transform barrel;
    private WeaponModelData modelData;

    private WeaponValues Stats => weaponParameters.weaponStats;

    public void SetParameters (WeaponParameters parameters) {
        weaponParameters = parameters;

        print(viewModel);

        if (viewModel != null) Destroy(viewModel);

        viewModel = Instantiate(weaponParameters.viewModel, transform, false);

        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;
    }

    private void Start() {
        mainCamera = Camera.main;

        SetParameters(weaponParameters);
    }

    private void FixedUpdate() {
        animator.SetInteger("WeaponType", (int)Stats.weaponClass);
    }

    public void StandardFire() {

        var hitPosition = Vector3.zero; //Used to check if the player is actually looking somewhere.

        for (int i = 0; i < weaponParameters.weaponStats.bulletsPerShot; i++) {
            if (weaponParameters.weaponStats.projectileType == ProjectileType.Hitscan) {
                HitScan(hitPosition);
            }
            else {
                Projectile(hitPosition);
            }
        }
    }

    void Projectile(Vector3 hitPosition) {

        if (barrel != null) {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 10000, raycastMask)) {
                hitPosition = hit.point;
            }
            if (hitPosition == Vector3.zero) {
                hitPosition = mainCamera.transform.position + mainCamera.transform.forward * 1000;
            }
        }

        barrel.LookAt(hitPosition);

        var bulletObject = Instantiate(Stats.Projectile, barrel.position, barrel.rotation) as GameObject;

        var SpreadX = Random.Range(-Stats.weaponSpread, Stats.weaponSpread);
        var SpreadY = Random.Range(-Stats.weaponSpread, Stats.weaponSpread);

        bulletObject.transform.Rotate(SpreadX, SpreadY, 0);
    }

    void HitScan(Vector3 hitPosition) {

        Transform hitParent = null;
        Rigidbody hitObjectRigid = null; //Rigidbody of object if it has one.

        var spreadVector = Vector3.zero;
        spreadVector += mainCamera.transform.right.normalized * (Random.Range(-Stats.weaponSpread, Stats.weaponSpread) / 100);
        spreadVector += mainCamera.transform.up.normalized * (Random.Range(-Stats.weaponSpread, Stats.weaponSpread) / 100);

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward + spreadVector, out RaycastHit hit, 10000, raycastMask)) {
            hitPosition = hit.point;
            hitParent = hit.transform;
            hitObjectRigid = hit.transform.GetComponent<Rigidbody>();
        }

        if (hitPosition != Vector3.zero) {

            Instantiate(impact, hitPosition, new Quaternion(0, 0, 0, 0), hitParent);

            if (hitObjectRigid != null) {
                hitObjectRigid.AddForceAtPosition((hitPosition - transform.position).normalized * 100f, hitPosition);
            }
        }
    }

    void ChargeWeapon() {
        if (chargeProgress < 100) {
            chargeProgress += Time.deltaTime * Stats.chargeRate;
        }
    }

    void FireGun(float trueFireRate) {
        burstCount = 0;
        loadTimer = trueFireRate;
        fireStage = FireStage.Firing;
    }

    private void Update() {
        Stats.bulletsPerShot = Mathf.Clamp(Stats.bulletsPerShot, 1, int.MaxValue);
        Stats.fireRate = Mathf.Clamp(Stats.fireRate, 1, int.MaxValue);
        Stats.bulletsPerBurst = Mathf.Clamp(Stats.bulletsPerBurst, 1, int.MaxValue);

        float trueFireRate = (1 / Stats.fireRate) + (Stats.burstTime * Stats.bulletsPerBurst);

        if (loadTimer <= 0) {
            if (Stats.weaponType == WeaponType.FullAuto) {
                if (Input.GetButton("Fire1")) {
                    FireGun(trueFireRate);
                }
            }
            else if (Stats.weaponType == WeaponType.Charge) {
                if (Input.GetButton("Fire1")) {
                    ChargeWeapon();
                    fireStage = FireStage.Charging;
                }
                if (fireStage == FireStage.Charging) {
                    if (Input.GetButtonUp("Fire1")) {
                        FireGun(trueFireRate);
                        chargeProgress = 0;
                    }
                    chargeCircle.fillAmount = chargeProgress / 100;
                }
            }
            else {
                if (Input.GetButtonDown("Fire1")) {
                    FireGun(trueFireRate);
                }
            }
        }
        else {
            loadTimer -= Time.deltaTime;
        }

        if (fireStage == FireStage.Firing) {
            if (burstCount < Stats.bulletsPerBurst) {
                if (burstTimer <= 0) {
                    burstCount += 1;
                    burstTimer = Stats.burstTime;
                    StandardFire();
                }
                else {
                    burstTimer -= Time.deltaTime;
                }
            }
            else {
                fireStage = FireStage.Idle;
            }
        }
    }
}
