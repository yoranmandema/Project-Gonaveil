using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocity;
    public float damage;
    public float decayTime;
    public GameObject impactObject;
    public Rigidbody bulletRigid;
    public Transform barrel;
    public Transform effect;
    Vector3 offset;

    void Start()
    {
        effect.position = barrel.position;
        offset = effect.localPosition;
        Destroy(gameObject, decayTime);
    }

    // Update is called once per frame
    void Update()
    {
        effect.localPosition = Vector3.Lerp(effect.localPosition, Vector3.zero, 10f * Time.deltaTime);
        bulletRigid.velocity = transform.forward * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        effect.localPosition = Vector3.zero;
        Instantiate(impactObject, collision.contacts[0].point, new Quaternion(0, 0, 0, 0), collision.transform);
        Destroy(gameObject);
    }
}
