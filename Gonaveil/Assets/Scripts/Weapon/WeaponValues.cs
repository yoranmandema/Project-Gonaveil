using UnityEngine;

public enum WeaponType { FullAuto, SemiAuto, Charge }
public enum WeaponClass { None, Pistol, Rifle }
public enum ProjectileType { Projectile, Hitscan }


[System.Serializable]
public class WeaponValues {
    public string WeaponName;
    //public GameObject modelObject;
    public int magazineCapacity;
    public float reloadTime;
    public float fireRate;
    public float weaponSpread;
    public float bulletsPerShot;
    public float bulletsPerBurst;
    public float burstTime;
    public float chargeTime;
    public bool fireWhenCharged;
    public GameObject Projectile;
    //public Transform Barrel;
    public WeaponType weaponType;
    public WeaponClass weaponClass;
    public ProjectileType projectileType;
}