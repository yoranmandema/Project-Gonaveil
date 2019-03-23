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

    public void SetParameters(WeaponParameters parameters) {
        if (viewModel != null) Destroy(viewModel);
        if (worldModel != null) Destroy(worldModel);

        weaponParameters = parameters;
 
        //creates new view model and world model.
        viewModel = Instantiate(weaponParameters.viewModel, transform, false);
        worldModel = Instantiate(weaponParameters.worldModel, handBone, false);
        //apply world model transformations
        worldModel.transform.localEulerAngles = new Vector3(-90, 90, 0);

        //get the model data for the barrel.
        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
