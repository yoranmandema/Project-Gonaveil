using System.Collections;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour, IPickup {

    public GameObject weapon;
    public float rotationSpeed = 1f;
    public LayerMask physicsMask;
    public GameObject owner;

    private GameObject worldModel;
    private WeaponSystem weaponComponent;


    void Start() {
        Initiate();
    }

    public void Initiate (GameObject newOwner = null) {
        if (worldModel) Destroy(worldModel);

        weapon.SetActive(false);

        weaponComponent = weapon.GetComponent<WeaponSystem>();

        worldModel = Instantiate(weaponComponent.worldModel, transform);

        if (newOwner != null) {
            owner = newOwner;

            DisableCollisionsWithPlayer(owner);
        }
    }

    void Update() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider) {
        var player = collider.gameObject.transform.root.gameObject;

        Pickup(player);
    }


    private void Pickup (GameObject player, bool replace = false) {
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
        else if (replace) {
            var weaponTransform = weapon.transform;

            SetWeapon(inventory.CurrentWeapon);

            inventory.AddWeapon(weaponTransform);

            Initiate(inventory.gameObject);
        }
    }

    public void DisableCollisionsWithPlayer(GameObject player) {
        IEnumerator Timer() {
            var colliders = owner.GetComponentsInChildren<Collider>();

            foreach (var collider in colliders) Physics.IgnoreCollision(collider, GetComponent<Collider>(), true);

            yield return new WaitForSeconds(1f);

            foreach (var collider in colliders) Physics.IgnoreCollision(collider, GetComponent<Collider>(), false);           
        }

        StartCoroutine(Timer());
    }

    public void SetWeapon (GameObject newWeapon) {
        weapon = newWeapon;

        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }

    public void OnPickup (PlayerInteract playerInteract) {
        var player = playerInteract.gameObject.transform.root.gameObject;

        Pickup(player, true);
    }
}
