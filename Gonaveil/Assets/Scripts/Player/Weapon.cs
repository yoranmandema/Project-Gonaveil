using UnityEngine;

public class Weapon : MonoBehaviour {
    public string Name;
    public WeaponType weaponType;
    public GameObject worldModel;
    public GameObject viewModel;
    public int magazineCapacity;
    public float reloadTime;
    public GameObject impact;

    private Camera mainCamera;

    public enum WeaponType { None, Pistol, Rifle, Throwable, Melee }

    private void Start() {
        mainCamera = Camera.main;
    }

    public void PrimaryFire() {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit)) {
            Instantiate(impact, hit.point, new Quaternion(0, 0, 0, 0), hit.transform);
            hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * 100f, hit.point);
        }
    }

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            PrimaryFire();
        }
    }
}
