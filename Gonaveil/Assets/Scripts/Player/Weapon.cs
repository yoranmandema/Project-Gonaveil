using UnityEngine;

public class Weapon : MonoBehaviour {
    public string Name;
    public WeaponType weaponType;
    public GameObject worldModel;
    public GameObject viewModel;
    public int magazineCapacity;
    public float reloadTime;
    public float fireRate;
    public float weaponSpread;
    public float bulletsPerShot;
    public GameObject impact;

    private Camera mainCamera;

    public enum WeaponType { None, Pistol, Rifle, Throwable, Melee }

    private void Start() {
        if (bulletsPerShot <= 0)
        {
            bulletsPerShot = 1;
        }
        if(fireRate <= 0)
        {
            fireRate = 1;
        }
        mainCamera = Camera.main;
    }

    public void PrimaryFire() {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 HitPosition = new Vector3(); //Used to check if the player is actually looking somewhere.
            Transform HitParent = null;
            Rigidbody HitObjectRigid = null; //Rigidbody of object if it has one.
            Vector3 SpreadVector = new Vector3();
            SpreadVector += mainCamera.transform.right.normalized * Random.Range(-weaponSpread, weaponSpread);
            SpreadVector += mainCamera.transform.up.normalized * Random.Range(-weaponSpread, weaponSpread);
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward + SpreadVector, out RaycastHit hit))
            {
                HitPosition = hit.point;
                HitParent = hit.transform;
                HitObjectRigid = hit.transform.GetComponent<Rigidbody>();
            }
            if (HitPosition != Vector3.zero)
            {
                Instantiate(impact, HitPosition, new Quaternion(0, 0, 0, 0), HitParent);
                if (HitObjectRigid != null)
                {
                    HitObjectRigid.AddForceAtPosition((HitPosition - transform.position).normalized * 100f, HitPosition);
                }
            }
        }
    }

    float LoadTime = 0;
    private void Update() {
        float TrueFireRate = 1 / fireRate;
        if (LoadTime <= 0)
        {
            if (Input.GetButton("Fire1"))
            {
                LoadTime = TrueFireRate;
                PrimaryFire();
            }
        }
        else
        {
            LoadTime -= Time.deltaTime;
        }

    }
}
