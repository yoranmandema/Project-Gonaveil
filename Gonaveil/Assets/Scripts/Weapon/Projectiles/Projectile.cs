using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float lifeTime = 10f;
    public float startVelocity = 10f;
    public float radius = 0.07f;
    public int timeSteps = 6; // Should be moved to some kind of settings file?

    public LayerMask mask;

    public Vector3 velocity;

    private float startTime;

    void Start()
    {
        velocity = transform.forward * startVelocity;

        startTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        SimulateSteps();

        if ((Time.realtimeSinceStartup - startTime) > lifeTime) {
            Destroy(gameObject);
        }
    }

    void SimulateSteps () {
        var position = transform.position;

        for (int i = 0; i < timeSteps; i++) {
            position = Simulate(position);
        }

        Debug.DrawLine(transform.position, position, Color.red, 10f);

        transform.position = position;
    }

    Vector3 Simulate (Vector3 start) {
        var deltaTime = Time.deltaTime / timeSteps;
        var position = start;

        position += velocity * deltaTime;

        var sphereCast = Physics.SphereCast(position, 0.07f, velocity, out RaycastHit hit, velocity.magnitude * deltaTime, mask);

        if (sphereCast) {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 10f);

            velocity = Vector3.Reflect(velocity.normalized, hit.normal) * velocity.magnitude;

            position = hit.point + velocity * deltaTime;
        }
        else {
            velocity += Physics.gravity * deltaTime;
        }

        return position;
    }
}
