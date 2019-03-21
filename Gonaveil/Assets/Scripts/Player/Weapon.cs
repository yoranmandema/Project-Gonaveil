using UnityEngine;

public class Weapon : MonoBehaviour {
    //this already looks like hell NOW TO ADD PROJECTILES
    [System.Serializable]
    public class WeaponValues
    {
        public int magazineCapacity;
        public float reloadTime;
        public float fireRate;
        public float weaponSpread;
        public float bulletsPerShot;
        public float bulletsPerBurst;
        public float burstTime;
        public float chargeRate;
        public GameObject Projectile;
        public Transform Barrel;
        public UnityEngine.UI.Image chargeCircle;
    }

    public WeaponValues weaponValues;
    public string Name;
    public WeaponType weaponType;
    public WeaponClass weaponClass;
    public ProjectileType projectileType;
    public LayerMask raycastMask;
    public GameObject worldModel;
    public GameObject viewModel;
    public GameObject impact;
    public Animator animator;

    private Camera mainCamera;

    public enum WeaponType { FullAuto, SemiAuto, Charge }
    public enum WeaponClass { None, Pistol, Rifle }
    public enum ProjectileType { Projectile, Hitscan}

    private void Start() {
        mainCamera = Camera.main;
    }

    private void FixedUpdate() {
        animator.SetInteger("WeaponType", (int)weaponClass);
    }

    public void StandardFire() {
        Vector3 hitPosition = new Vector3(); //Used to check if the player is actually looking somewhere.
        for (int i =0; i < weaponValues.bulletsPerShot;i++)
        {
            if(projectileType == ProjectileType.Hitscan)
            {
                HitScan(hitPosition);
            }
            else
            {
                Projectile(hitPosition);
            }
        }
    }

    void Projectile(Vector3 hitPosition)
    {
        if (weaponValues.Barrel != null)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 10000, raycastMask))
            {
                hitPosition = hit.point;
            }
            if (hitPosition == Vector3.zero)
            {
                hitPosition = mainCamera.transform.position + mainCamera.transform.forward * 1000;
            }
        }
        weaponValues.Barrel.LookAt(hitPosition);
        GameObject bulletObject = Instantiate(weaponValues.Projectile, weaponValues.Barrel.position, weaponValues.Barrel.rotation) as GameObject;
        float SpreadX = Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread);
        float SpreadY = Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread);
        bulletObject.transform.Rotate(SpreadX, SpreadY, 0);
    }

    void HitScan(Vector3 hitPosition)
    {
        Transform hitParent = null;
        Rigidbody hitObjectRigid = null; //Rigidbody of object if it has one.
        Vector3 spreadVector = new Vector3();
        spreadVector += mainCamera.transform.right.normalized * (Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread) / 100);
        spreadVector += mainCamera.transform.up.normalized * (Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread) / 100);
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward + spreadVector, out RaycastHit hit, 10000, raycastMask))
        {
            hitPosition = hit.point;
            hitParent = hit.transform;
            hitObjectRigid = hit.transform.GetComponent<Rigidbody>();
        }
        if (hitPosition != Vector3.zero)
        {
            Instantiate(impact, hitPosition, new Quaternion(0, 0, 0, 0), hitParent);
            if (hitObjectRigid != null)
            {
                hitObjectRigid.AddForceAtPosition((hitPosition - transform.position).normalized * 100f, hitPosition);
            }
        }
    }

    void ChargeWeapon()
    {
        if(chargeProgress < 100)
        {
            chargeProgress += Time.deltaTime * weaponValues.chargeRate;
        }
    }

    void FireGun(float trueFireRate)
    {
        burstCount = 0;
        loadTimer = trueFireRate;
        fireStage = FireStage.Firing;
    }

    float loadTimer = 0;
    float burstTimer = 0;
    public float chargeProgress = 0;
    float burstCount = 0;
    public enum FireStage { Idle, Firing, Charging}
    FireStage fireStage;
    private void Update() {
        weaponValues.bulletsPerShot = Mathf.Clamp(weaponValues.bulletsPerShot, 1, 934157136952);
        weaponValues.fireRate = Mathf.Clamp(weaponValues.fireRate, 1, 934157136952);
        weaponValues.bulletsPerBurst = Mathf.Clamp(weaponValues.bulletsPerBurst, 1, 934157136952);
        float trueFireRate = (1 / weaponValues.fireRate) + (weaponValues.burstTime * weaponValues.bulletsPerBurst);
        if (loadTimer <= 0)
        {
            if (weaponType == WeaponType.FullAuto)
            {
                if (Input.GetButton("Fire1"))
                {
                    FireGun(trueFireRate);
                }
            }else if(weaponType == WeaponType.Charge)
            {
                if (Input.GetButton("Fire1"))
                {
                    ChargeWeapon();
                    fireStage = FireStage.Charging;
                }
                if(fireStage == FireStage.Charging)
                {
                    if (Input.GetButtonUp("Fire1"))
                    {
                        FireGun(trueFireRate);
                        chargeProgress = 0;
                    }
                    weaponValues.chargeCircle.fillAmount = chargeProgress / 100;
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    FireGun(trueFireRate);
                }
            }
        }
        else
        {
            loadTimer -= Time.deltaTime;
        }
        if (fireStage == FireStage.Firing)
        {
            if (burstCount < weaponValues.bulletsPerBurst)
            {
                if (burstTimer <= 0)
                {
                    burstCount += 1;
                    burstTimer = weaponValues.burstTime;
                    StandardFire();
                }
                else
                {
                    burstTimer -= Time.deltaTime;
                }
            }
            else
            {
                fireStage = FireStage.Idle;
            }
        }
    }
}
