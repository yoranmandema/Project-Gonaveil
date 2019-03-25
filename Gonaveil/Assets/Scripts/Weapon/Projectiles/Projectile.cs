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
    public float timeScale = 1f;

    public LayerMask mask;
    public Vector3 velocity;
    public GameObject instigator;
    public Transform barrel;
    public Transform effect;
    public Weapon weapon;

    public bool hasContact;

    private float startTime;


    public void Fire() {
        effect.position = barrel.position;

        OnStart();

        startTime = Time.realtimeSinceStartup;
    }

    void FixedUpdate() {
        SimulateSteps();

        OnUpdate();

        //move the effect slowly to the centre of the actual bullet.
        effect.localPosition = Vector3.Lerp(effect.localPosition, Vector3.zero, 10f * Time.fixedDeltaTime);

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
        var deltaTime = Time.fixedDeltaTime / timeSteps * timeScale;
        var position = start;
        var hit = default(RaycastHit);

        position += velocity * deltaTime;

        if (useSphereCast) {
            hasContact = Physics.SphereCast(position, radius, velocity, out hit, velocity.magnitude * deltaTime, mask);
        }
        else {
            hasContact = Physics.Raycast(position, velocity, out hit, velocity.magnitude * deltaTime, mask);
        }

        if (hasContact) {
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 10f);

            OnHit(ref position, deltaTime, hit);

            //force bullet effect to centre
            effect.localPosition = Vector3.zero;
        }
        else {
            OnSimulateGravity(ref position, deltaTime);
        }

        OnSimulate(ref position, deltaTime);

        return position;
    }

    public virtual void OnHit(ref Vector3 position, float deltaTime, RaycastHit hit) { }
    public virtual void OnSimulate(ref Vector3 position, float deltaTime) { }
    public virtual void OnSimulateGravity(ref Vector3 position, float deltaTime) {
        velocity += Physics.gravity * gravityScale * deltaTime;
    }
    public virtual void OnUpdate() { }
    public virtual void OnStart() {
        velocity = transform.forward * startVelocity;
    }
}

