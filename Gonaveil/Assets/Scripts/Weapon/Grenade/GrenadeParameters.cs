using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewGrenadeParameters", menuName = "Weapons/New Grenade Parameters")]
public class GrenadeParameters : ScriptableObject {
    public GameObject prefab;
}
