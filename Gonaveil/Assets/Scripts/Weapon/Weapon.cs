using UnityEngine;

public class Weapon : MonoBehaviour {
    public PlayerInputController controller;
    public UnityEngine.UI.Image chargeCircle;
    public WeaponParameters weaponParameters;
    public LayerMask raycastMask;
    public GameObject impact;
    public Animator animator;
    public Transform handBone;
    //public bool disabled;
    public bool weaponEquipped;

    public enum FireStage { Idle, Firing, Charging, Cycling, Reloading}
    public float chargeProgress;
    public GameObject viewModel;
    public GameObject worldModel;
    public int currentMagazine;
    public int currentAmmoPool;

    private Camera mainCamera;
    private float loadTimer;
    private float burstTimer;
    private float burstCount;
    private FireStage fireStage;
    private Transform barrel;
    private WeaponModelData modelData;

    private int playerLayer;

    private WeaponValues Stats => weaponParameters.weaponStats;

    public void SetParameters (WeaponParameters parameters) {
        //applies the selected weapon's parameters
        weaponParameters = parameters;

        //removes previous view model and world model
        if (viewModel != null) Destroy(viewModel);
        if (worldModel != null) Destroy(worldModel);
        fireStage = FireStage.Idle;

        //creates new view model and world model.
        viewModel = Instantiate(weaponParameters.viewModel, transform, false);
        worldModel = Instantiate(weaponParameters.viewModel, handBone, false);
        //apply world model transformations
        worldModel.transform.localEulerAngles = new Vector3(-90, 90, 0);
        worldModel.layer = playerLayer;
        //force world model to the player layer
        foreach (var mesh in worldModel.GetComponentsInChildren<MeshRenderer>()) {
            mesh.gameObject.layer = playerLayer;
        }

        //get the model data for the barrel.
        modelData = viewModel.GetComponent<WeaponModelData>();
        barrel = modelData.barrel;
    }

    public void SetWeaponAmmo(int magazine, int ammoPool) //Used to set the ammo. Can be called anywhere.
    {
        currentMagazine = magazine;
        currentAmmoPool = ammoPool;
    }

    public void Disarm() //Used by the inventory system to ensure the player can't shoot when there are no more weapons
    {
        weaponEquipped = false;

        Destroy(viewModel);
        Destroy(worldModel);
    }

    public void Rearm() //return the system to be active again.
    {
        weaponEquipped = true;
    }

    private void Start() {
        //sets the camera
        mainCamera = Camera.main;
        //force the player into the layer
        playerLayer = LayerMask.NameToLayer("Player");
        //set the weapon to the one already in.
        SetParameters(weaponParameters);
    }

    private void FixedUpdate() {
        //animation stuff, ask Galaxy.
        animator.SetInteger("WeaponType", (int)Stats.weaponClass);
    }

    public void WeaponFire() {
        //remove a round from the magazine
        currentMagazine -= 1;
        try //Placeholder until we add muzzle flashes for all weapons.
        {
            transform.GetComponentInChildren<ParticleSystem>().Play(true);
        }
        catch
        {

        }
        //loops to fire multiple shots in one round
        for (int i = 0; i < weaponParameters.weaponStats.bulletsPerShot; i++) {
            //check if the gun is hitscan or projectile, does the apporpriate thing.
            if (weaponParameters.weaponStats.projectileType == ProjectileType.Hitscan) {
                HitScan();
            }
            else {
                Projectile();
            }
        }
    }

    void Projectile() {

        //create bullet
        var projectileObject = Instantiate(Stats.Projectile, mainCamera.transform.position, mainCamera.transform.rotation) as GameObject;
        
        //tell the bullet where the barrel is (used for bullet model)
        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.barrel = barrel;
        projectile.instigator = transform.root.gameObject;
        projectile.Fire();

        //calculate uniform spread
        var angle = Random.Range(0, 2 * Mathf.PI);
        var offset = Random.Range(0, Stats.weaponSpread);
        var SpreadX = Mathf.Cos(angle) * offset;
        var SpreadY = Mathf.Sin(angle) * offset;
        //apply spread
        projectileObject.transform.Rotate(SpreadX, SpreadY, 0);
    }

    void HitScan() {
        var hitPosition = Vector3.zero; //Used to check if the player is actually looking somewhere.
        Transform hitParent = null;
        Rigidbody hitObjectRigid = null; //Rigidbody of object if it has one.

        //vector to apply to raycast
        var spreadVector = Vector3.zero;

        //calculate uniform spread
        var angle = Random.Range(0, 2 * Mathf.PI);
        var offset = Random.Range(0, Stats.weaponSpread);

        //Apply vector maths to ensure the raycast moves.
        spreadVector += mainCamera.transform.right.normalized * Mathf.Cos(angle) * offset;
        spreadVector += mainCamera.transform.up.normalized * Mathf.Sin(angle) * offset;
        spreadVector += mainCamera.transform.forward * 75;

        //fire a raycast in that direction.
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward + spreadVector, out RaycastHit hit, 10000, raycastMask)) {
            hitPosition = hit.point;
            hitParent = hit.transform;
            hitObjectRigid = hit.transform.GetComponent<Rigidbody>();
        }

        //if there is no hit, don't bother calculating impacts.
        if (hitPosition != Vector3.zero) {

            //spawn impact effect on wall
            Instantiate(impact, hitPosition, new Quaternion(0, 0, 0, 0), hitParent);

            //check if hit object is a rigidbody, apply a force.
            if (hitObjectRigid != null) {
                hitObjectRigid.AddForceAtPosition((hitPosition - transform.position).normalized * 100f, hitPosition);
            }
        }
    }

    void ChargeWeapon() {
        //just increments the charge progress until 1
        if (chargeProgress < 1) {
            chargeProgress = Mathf.Min(1, chargeProgress + Time.deltaTime / Stats.chargeTime);
        }
    }

    void PrepareFire(float trueFireRate) { //pulls the trigger
        burstCount = 0;
        loadTimer = trueFireRate;
        fireStage = FireStage.Firing;
    }
    
    void ReloadGun()
    {
        if (currentAmmoPool > 0)
        {
            Debug.Log("reloadin");
            //increments loadTimer until = reload time
            if (loadTimer < Stats.reloadTime)
            {
                loadTimer += Time.deltaTime;
            }
            else
            {
                //set values and reset the gun.
                currentAmmoPool += currentMagazine;
                if (currentAmmoPool < Stats.magazineCapacity)
                {
                    currentMagazine = currentAmmoPool;
                }
                else
                {
                    currentMagazine = Stats.magazineCapacity;
                }
                currentAmmoPool -= currentMagazine;
                loadTimer = 0;
                fireStage = FireStage.Idle;
            }
        }
    }

    void CycleGun(float trueFireRate)
    {
        //Simulates fire rate, inputs aren't detected until cycled
        if (loadTimer <= 0)
        {
            //Check if the player is using primary fire
            if (controller.triggerState == PlayerInputController.TriggerStates.Primary)
            {
                //Full Auto, "pulls" trigger every time the gun has cycled
                if (Stats.weaponType == WeaponType.FullAuto)
                {
                    PrepareFire(trueFireRate);
                }
                else if (Stats.weaponType == WeaponType.Charge) //Charge weapons are similar. "Holds" trigger.
                {
                    ChargeWeapon();
                    fireStage = FireStage.Charging;
                }
                else
                {
                    if (fireStage != FireStage.Cycling) //semi auto is a different system, this checks if the gun hasn't fired.
                    {
                        PrepareFire(trueFireRate);
                    }
                }
            }
            //Checks if the gun is charging
            if (fireStage == FireStage.Charging)
            {
                if (controller.triggerState == PlayerInputController.TriggerStates.Idle || (Stats.fireWhenCharged && chargeProgress == 1)) //waits for trigger release or when charging is done
                {
                    PrepareFire(trueFireRate); //fires gun
                    chargeProgress = 0;
                }
                //UI circle
                chargeCircle.fillAmount = chargeProgress;
            }
                //waits for trigger to be released before allowing the player to fire again.
             if (controller.triggerState == PlayerInputController.TriggerStates.Idle)
             {
                fireStage = FireStage.Idle;
             }
        }
        else
        {
            //cycle gun
            loadTimer -= Time.deltaTime;
        }
    }

    void GunFireMechanics() //Handles burst fire and semi auto mechanics.
    {
        //check if the gun hasn't fired it's burst, and there's enough ammo
        if (burstCount < Stats.bulletsPerBurst && currentMagazine > 0)
        {
            //adds delay between bursts
            if (burstTimer <= 0)
            {
                //increments the burst counter and resets the timer
                burstCount += 1;
                burstTimer = Stats.burstTime;
                WeaponFire(); //Finally fire the gun
            }
            else
            {
                //decrease the timer.
                burstTimer -= Time.deltaTime;
            }
        }
        else
        {
            //used for semi auto, ensures the trigger has to be released to allow another shot.
            if (Stats.weaponType == WeaponType.SemiAuto)
            {
                fireStage = FireStage.Cycling;
            }
            else
            {
                fireStage = FireStage.Idle;
            }
        }
    }

    private void Update() {
        Debug.Log(fireStage);
        //boolean to tell if there is any weapon in the inventory
        if (weaponEquipped)
        {
            //clamps to ensure no errors
            Stats.bulletsPerShot = Mathf.Clamp(Stats.bulletsPerShot, 1, int.MaxValue);
            Stats.fireRate = Mathf.Clamp(Stats.fireRate, 1, int.MaxValue);
            Stats.bulletsPerBurst = Mathf.Clamp(Stats.bulletsPerBurst, 1, int.MaxValue);

            //calculate from RPM to Cycle time.
            float trueFireRate = (1 / (Stats.fireRate / 60)) + (Stats.burstTime * Stats.bulletsPerBurst);

            //check if the gun hasn't begun reloading
            if (fireStage != FireStage.Reloading)
            {
                //check if the player wants to reload and they can, or if the gun is in dire need of a reload.
                CycleGun(trueFireRate);
                if (fireStage == FireStage.Firing) //check if the gun should be firing
                {
                    GunFireMechanics();
                }
                if (((InputManager.GetButtonDown("Reload Weapon") && currentMagazine < Stats.magazineCapacity) || currentMagazine <= 0))
                {
                    //loadTimer is reused for reloading, saves memory space :)
                    loadTimer = 0;
                    //force reload
                    fireStage = FireStage.Reloading;
                }
            }
            else
            {
                //RELOAD
                ReloadGun();
            }
        }
    }
}
