using UnityEngine;

public class Weapon : MonoBehaviour {

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
    }

    public WeaponValues weaponValues;
    public string Name;
    public WeaponType weaponType;
    public GameObject worldModel;
    public GameObject viewModel;

    public GameObject impact;

    private Camera mainCamera;

    public enum WeaponType { FullAuto, SemiAuto, Charge}

    private void Start() {
        mainCamera = Camera.main;
    }

    public void StandardFire() {
        
        for(int i =0; i < weaponValues.bulletsPerBurst;i++)
        {
                Vector3 hitPosition = new Vector3(); //Used to check if the player is actually looking somewhere.
                Transform hitParent = null;
                Rigidbody hitObjectRigid = null; //Rigidbody of object if it has one.
                Vector3 spreadVector = new Vector3();
                spreadVector += mainCamera.transform.right.normalized * Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread);
                spreadVector += mainCamera.transform.up.normalized * Random.Range(-weaponValues.weaponSpread, weaponValues.weaponSpread);
                if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward + spreadVector, out RaycastHit hit))
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
    }

    float loadTimer = 0;
    public float burstTimer = 0;
    public float burstCount = 0;
    bool Firing;
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
                    burstCount = 0;
                    loadTimer = trueFireRate;ut
                    Firing = true;
                }
            }else if(weaponType == WeaponType.Charge)
            {

            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    burstCount = 0;
                    loadTimer = trueFireRate;
                    Firing = true;
                }
            }
        }
        else
        {
            loadTimer -= Time.deltaTime;
        }
        if (Firing)
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
                
                Firing = false;
            }
        }
    }
}
