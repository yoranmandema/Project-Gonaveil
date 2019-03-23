using UnityEngine;

public class Weapon : MonoBehaviour {
    public PlayerInputController controller;
    public UnityEngine.UI.Image chargeCircle;
    public WeaponParameters weaponParameters;
    public LayerMask raycastMask;
    public GameObject impact;
    public Animator animator;
    public Transform handBone;
    public bool disabled;

    public enum FireStage { Idle, Firing, Charging, Cycled}
    public float chargeProgress;
    public GameObject viewModel;
    public GameObject worldModel;

    private Camera mainCamera;
    private float loadTimer;
    private float burstTimer;
    private float burstCount;
    private FireStage fireStage;
    private Transform barrel;
    private WeaponModelData modelData;
    private int playerLayer;

    private WeaponValues Stats => weaponParameters.weaponStats;

    public void SetParameters (WeaponParameters parameters) {
        weaponParameters = parameters;

        if (viewModel != null) Destroy(viewModel);
        if (worldModel != null) Destroy(worldModel);

        viewModel = Instantiate(weaponParameters.viewModel, transform, false);
        worldModel = Instantiate(weaponParameters.viewModel, handBone, false);
        worldModel.transform.localEulerAngles = new Vector3(-90, 90, 0);
        worldModel.layer = playerLayer;
        foreach (var mesh in worldModel.GetComponentsInChildren<MeshRenderer>()) {
            mesh.gameObject.layer = playerLayer;
        }

        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;
    }

    public void Disarm()
    {
        if (disabled) return;

        disabled = true;
        Destroy(viewModel);
        Destroy(worldModel);
    }

    public void Rearm()
    {
        disabled = false;
    }

    private void Start() {
        mainCamera = Camera.main;
        playerLayer = LayerMask.NameToLayer("Player");

        SetParameters(weaponParameters);
    }

    private void FixedUpdate() {
        animator.SetInteger("WeaponType", (int)Stats.weaponClass);
    }

    public void WeaponFire() {

        for (int i = 0; i < weaponParameters.weaponStats.bulletsPerShot; i++) {
            if (weaponParameters.weaponStats.projectileType == ProjectileType.Hitscan) {
                HitScan();
            }
            else {
                Projectile();
            }
        }
    }

    void Projectile() {

        var bulletObject = Instantiate(Stats.Projectile, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;
        bulletObject.GetComponent<Bullet>().barrel = barrel;
        var angle = Random.Range(0, 2 * Mathf.PI);
        var offset = Random.Range(0, Stats.weaponSpread);
        var SpreadX = Mathf.Cos(angle) * offset;
        var SpreadY = Mathf.Sin(angle) * offset;

        bulletObject.transform.Rotate(SpreadX, SpreadY, 0);
    }

    void HitScan() {
        var hitPosition = Vector3.zero; //Used to check if the player is actually looking somewhere.
        Transform hitParent = null;
        Rigidbody hitObjectRigid = null; //Rigidbody of object if it has one.

        var spreadVector = Vector3.zero;

        var angle = Random.Range(0, 2 * Mathf.PI);
        var offset = Random.Range(0, Stats.weaponSpread);
        spreadVector += mainCamera.transform.right.normalized * Mathf.Cos(angle) * offset;
        spreadVector += mainCamera.transform.up.normalized * Mathf.Sin(angle) * offset;
        spreadVector += mainCamera.transform.forward * 75;

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
        if (chargeProgress < 1) {
            chargeProgress = Mathf.Min(1, chargeProgress + Time.deltaTime / Stats.chargeTime);
        }
    }

    void FireGun(float trueFireRate) {
        burstCount = 0;
        loadTimer = trueFireRate;
        fireStage = FireStage.Firing;
        try
        {
            transform.GetComponentInChildren<ParticleSystem>().Play(true);
        }
        catch
        {

        }
    }

    private void Update() {
        if (!disabled)
        {
            Stats.bulletsPerShot = Mathf.Clamp(Stats.bulletsPerShot, 1, int.MaxValue);
            Stats.fireRate = Mathf.Clamp(Stats.fireRate, 1, int.MaxValue);
            Stats.bulletsPerBurst = Mathf.Clamp(Stats.bulletsPerBurst, 1, int.MaxValue);

            float trueFireRate = (1 / (Stats.fireRate / 60)) + (Stats.burstTime * Stats.bulletsPerBurst);

            if (loadTimer <= 0)
            {
                if (controller.triggerState == PlayerInputController.TriggerStates.Primary)
                {
                    if (Stats.weaponType == WeaponType.FullAuto)
                    {
                        FireGun(trueFireRate);
                    }
                    else if (Stats.weaponType == WeaponType.Charge)
                    {
                        ChargeWeapon();
                        fireStage = FireStage.Charging;
                    }
                    else
                    {
                        if (fireStage != FireStage.Cycled)
                        {
                            FireGun(trueFireRate);
                        }
                    }
                }
                if (fireStage == FireStage.Charging)
                {
                    if (controller.triggerState == PlayerInputController.TriggerStates.Idle || (Stats.fireWhenCharged && chargeProgress == 1))
                    {
                        FireGun(trueFireRate);
                        chargeProgress = 0;
                    }
                    chargeCircle.fillAmount = chargeProgress;
                }
                else if (fireStage == FireStage.Cycled)
                {
                    if (controller.triggerState == PlayerInputController.TriggerStates.Idle)
                    {
                        fireStage = FireStage.Idle;
                    }
                }
            }
            else
            {
                loadTimer -= Time.deltaTime;
            }

            if (fireStage == FireStage.Firing)
            {
                if (burstCount < Stats.bulletsPerBurst)
                {
                    if (burstTimer <= 0)
                    {
                        burstCount += 1;
                        burstTimer = Stats.burstTime;
                        WeaponFire();
                    }
                    else
                    {
                        burstTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    if (Stats.weaponType == WeaponType.SemiAuto)
                    {
                        fireStage = FireStage.Cycled;
                    }
                    else
                    {
                        fireStage = FireStage.Idle;
                    }
                }
            }
        }
    }
}
