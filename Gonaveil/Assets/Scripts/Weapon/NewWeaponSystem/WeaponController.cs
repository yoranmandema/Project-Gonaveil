using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    public Transform weaponHolder;
    public Transform camera;
    public Transform handBone;

    public WeaponAsset weaponAsset;

    private GameObject viewModel;
    private GameObject worldModel;
    private Transform barrel;
    private WeaponModelData modelData;
    private InventorySystem inventorySystem;

    private WeaponComponent primaryComponent;
    private WeaponComponent secondaryComponent;

    public void SetParameters(WeaponAsset asset) {
        if (viewModel != null) Destroy(viewModel);
        if (worldModel != null) Destroy(worldModel);
        if (primaryComponent != null) Destroy(primaryComponent);
        if (secondaryComponent != null) Destroy(secondaryComponent);

        weaponAsset = asset;

        viewModel = Instantiate(weaponAsset.viewModel, transform, false);
        worldModel = Instantiate(weaponAsset.worldModel, handBone, false);

        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;

        // Add WeaponComponents to Weapon Holder object on player.
        primaryComponent = weaponHolder.gameObject.AddComponent(weaponAsset.primaryComponent.GetType()) as WeaponComponent;
        secondaryComponent = weaponHolder.gameObject.AddComponent(weaponAsset.secondaryComponent.GetType()) as WeaponComponent;
    }

    void Start() {
        inventorySystem = GetComponent<InventorySystem>();
    }

    void Update() {
        if (primaryComponent != null) {
            if (InputManager.GetButtonDown("Fire1"))
                primaryComponent.OnFireStart();
            else if (InputManager.GetButtonUp("Fire1"))
                primaryComponent.OnFireEnd();
        }

        if (secondaryComponent != null) {
            if (InputManager.GetButtonDown("Fire2"))
                secondaryComponent.OnFireStart();
            else if (InputManager.GetButtonUp("Fire2"))
                secondaryComponent.OnFireEnd();
        }
    }
}
