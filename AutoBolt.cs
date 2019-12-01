using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBolt : MonoBehaviour
{
    public float speed;


    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(Player.instance.normalDamage, transform.forward);
        }

        if (collision.gameObject.tag == "Minion")
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(25, transform.forward);
        }

        if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(25, transform.position);
            Destroy(gameObject);
        }

        //if (collision.gameObject.tag != "Player")
            if (collision.gameObject.tag != "Auto Crossbow")
                Destroy(gameObject);
    }

}
