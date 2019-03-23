using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float spawnDelay;
    public Transform spawnPosition;
    private GameObject spawnedWeapon;
    private float spawnTimer;
    public bool spawnOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnWeapon();
        }
    }

    void SpawnWeapon()
    {
        spawnTimer = spawnDelay;
        spawnedWeapon = Instantiate(weaponPrefab, spawnPosition.position, transform.rotation) as GameObject;
        spawnedWeapon.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnedWeapon == null)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer <= 0)
            {
                SpawnWeapon();
            }
        }
        else
        {
            spawnedWeapon.transform.Rotate(0, 50 * Time.deltaTime, 0);
        }
    }
}
