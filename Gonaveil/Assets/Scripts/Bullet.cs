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
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, decayTime);
    }

    // Update is called once per frame
    void Update()
    {
        bulletRigid.velocity = transform.forward * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(impactObject, collision.contacts[0].point, new Quaternion(0, 0, 0, 0), collision.transform);
        Destroy(gameObject);
    }
}
