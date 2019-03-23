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

    void Start()
    {
        //sets gets the bullet effect position to the barrel
        effect.position = barrel.position;
        //destroy the gameobject after a certain time.
        Destroy(gameObject, decayTime);
    }

    // Update is called once per frame
    void Update()
    {
        //move the effect slowly to the centre of the actual bullet.
        effect.localPosition = Vector3.Lerp(effect.localPosition, Vector3.zero, 10f * Time.deltaTime);
        //apply velocity
        bulletRigid.velocity = transform.forward * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //force bullet effect to centre
        effect.localPosition = Vector3.zero;
        //create impact
        Instantiate(impactObject, collision.contacts[0].point, new Quaternion(0, 0, 0, 0), collision.transform);
        //destroy bullet
        Destroy(gameObject);
    }
}
