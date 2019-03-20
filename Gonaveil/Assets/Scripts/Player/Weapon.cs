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
    public GameObject impact;

    private Camera mainCamera;

    public enum WeaponType { None, Pistol, Rifle, Throwable, Melee }

    private void Start() {
        mainCamera = Camera.main;
    }

    public void PrimaryFire() {
        Vector3 HitPosition = new Vector3(); //Used to check if the player is actually looking somewhere.
        Transform HitParent = null;
        Rigidbody HitObjectRigid = null; //Rigidbody of object if it has one.
        Vector3 SpreadVector = new Vector3();
        SpreadVector += mainCamera.transform.right.normalized * Random.Range(-weaponSpread, weaponSpread);
        SpreadVector += mainCamera.transform.up.normalized * Random.Range(-weaponSpread, weaponSpread);
        if (Physics.Raycast(mainCamera.transform.position + SpreadVector, mainCamera.transform.forward, out RaycastHit hit)) {
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

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            PrimaryFire();
        }
    }
}
