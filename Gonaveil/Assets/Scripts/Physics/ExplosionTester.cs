using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTester : MonoBehaviour
{
    public LayerMask mask;

    public float force;
    public float upwards;
    public float radius;

    public GamePlayPhysics.ExplosionType explosionType;

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, Mathf.Infinity,  mask)) {
                GamePlayPhysics.DoExplosion(hit.point, radius, force, upwards);
            }
        }
    }
}
