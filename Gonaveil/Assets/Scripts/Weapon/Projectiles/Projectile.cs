using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    public float lifeTime = 10f;
    public float startVelocity = 10f;
    public float gravityScale = 1f;

    public bool useSphereCast = false;
    public float radius = 0.07f;
    public int timeSteps = 6; // Should be moved to some kind of settings file?

    public LayerMask mask;
    public Vector3 velocity;
    public GameObject instigator;
    public Transform barrel;
    public Transform effect;

    private float startTime;

    public void Fire() {
        OnStart();

        startTime = Time.realtimeSinceStartup;
    }

    void Update() {
        SimulateSteps();

        OnUpdate();

        //move the effect slowly to the centre of the actual bullet.
        effect.localPosition = Vector3.Lerp(effect.localPosition, Vector3.zero, 10f * Time.deltaTime);

        if ((Time.realtimeSinceStartup - startTime) > lifeTime) {
            Destroy(gameObject);
        }
    }

    void SimulateSteps() {
        var position = transform.position;

        for (int i = 0; i < timeSteps; i++) {
            position = Simulate(position);
        }

        Debug.DrawLine(transform.position, position, Color.red, 10f);

        transform.position = position;
    }

    Vector3 Simulate(Vector3 start) {
        var deltaTime = Time.deltaTime / timeSteps;
        var position = start;
        var isHit = false;
        var hit = default(RaycastHit);

        position += velocity * deltaTime;

        if (useSphereCast) {
            isHit = Physics.SphereCast(position, radius, velocity, out hit, velocity.magnitude * deltaTime, mask);
        }
        else {
            isHit = Physics.Raycast(position, velocity, out hit, velocity.magnitude * deltaTime, mask);
        }

        if (isHit) {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 10f);

            OnHit(hit, ref position, deltaTime);

            //force bullet effect to centre
            effect.localPosition = Vector3.zero;
        }
        else {
            velocity += Physics.gravity * gravityScale * deltaTime;
        }

        OnSimulate(ref position, deltaTime);

        return position;
    }

    public virtual void OnHit(RaycastHit hit, ref Vector3 position, float deltaTime) { }
    public virtual void OnSimulate(ref Vector3 position, float deltaTime) { }
    public virtual void OnUpdate() { }
    public virtual void OnStart() {
        velocity = transform.forward * startVelocity;
    }
}

