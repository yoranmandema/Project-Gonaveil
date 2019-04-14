using UnityEngine;

public class DroppedWeapon : MonoBehaviour, IPickup {

    public GameObject weapon;
    public float rotationSpeed = 1f;

    private GameObject worldModel;
    private WeaponSystem weaponComponent;

    void Start() {
        weapon.SetActive(false);

        weaponComponent = weapon.GetComponent<WeaponSystem>();

        worldModel = Instantiate(weaponComponent.worldModel, transform);
    }

    void Update() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider) {
        var player = collider.gameObject.transform.root.gameObject;

        Pickup(player);
    }

    private void Pickup (GameObject player) {
        var inventory = player.GetComponent<WeaponInventory>();
        WeaponSystem inventoryWeaponSystem = null;

        if (inventory.CurrentWeapon != null) inventoryWeaponSystem = inventory.CurrentWeapon.GetComponent<WeaponSystem>();

        if (inventoryWeaponSystem == null) {
            inventory.AddWeapon(weapon.transform);

            Destroy(gameObject);
        }
        else if (weaponComponent.weaponName == inventoryWeaponSystem.weaponName) {
            //inventoryWeaponSystem.ammo

            Destroy(gameObject);
        }
        else if (inventory.IsSlotAvailable(0) || inventory.IsSlotAvailable(1)) {
            inventory.AddWeapon(weapon.transform);

            Destroy(gameObject);
        }
    }

    public void OnPickup (PlayerInteract playerInteract) {
        var player = playerInteract.gameObject.transform.root.gameObject;

        Pickup(player);
    }
}
