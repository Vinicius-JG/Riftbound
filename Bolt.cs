using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public float speed;
    bool launched;

    private void Start()
    {
        if (GameObject.Find("CrossbowShooter") != null)
            transform.parent = GameObject.Find("CrossbowShooter").transform;
    }

    private void Update()
    {
        if (launched)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.position = GameObject.Find("CrossbowShooter").transform.position;
        }
    }

    public void Launch()
    {
        transform.parent = null;
        launched = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && launched)
        {
            other.GetComponent<Enemy>().TakeDamage(Player.instance.normalDamage, transform.forward);
        }

        if(other.tag != "Player" && launched)
            Destroy(gameObject);
    }
}
