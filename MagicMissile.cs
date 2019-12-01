using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    Transform target;
    public float speed;

    void Start()
    {
        //transform.rotation = GameObject.Find("StaffShooter").transform.rotation;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (target != null)
            transform.LookAt(target);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(Player.instance.normalDamage, transform.forward);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
            target = other.transform;
    }

}
