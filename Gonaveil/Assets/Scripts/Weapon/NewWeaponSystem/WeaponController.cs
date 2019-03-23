using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform weaponHolder;
    public Transform camera;
    public Transform handBone;

    public WeaponParameters weaponParameters;

    private GameObject viewModel;
    private GameObject worldModel;
    private Transform barrel;
    private WeaponModelData modelData;
    private InventorySystem inventorySystem;

    public void SetParameters(WeaponParameters parameters) {
        if (viewModel != null) Destroy(viewModel);
        if (worldModel != null) Destroy(worldModel);

        weaponParameters = parameters;
 
        viewModel = Instantiate(weaponParameters.viewModel, transform, false);
        worldModel = Instantiate(weaponParameters.worldModel, handBone, false);

        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;
    }

    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>();
    }

    void Update()
    {
        
    }
}
