using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float startVelocity = 10f;
    public float radius = 0.07f;

    public LayerMask mask;

    public Vector3 velocity;

    void Start()
    {
        velocity = transform.forward * startVelocity;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + velocity * Time.deltaTime, Color.red, 100f);

        transform.position = transform.position + velocity * Time.deltaTime;

        var sphereCast = Physics.SphereCast(transform.position, 0.07f, velocity, out RaycastHit hit, velocity.magnitude * Time.deltaTime, mask);

        if (sphereCast) {
            velocity = Vector3.Reflect(velocity.normalized, hit.normal) * velocity.magnitude;

            transform.position = hit.point;
        }
        else {
            velocity += Physics.gravity * Time.deltaTime;
        }
    }
}
