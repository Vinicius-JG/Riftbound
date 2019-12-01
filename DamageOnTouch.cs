using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{

    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !anim.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle"))
        {
            other.GetComponent<Player>().TakeDamage(15, transform.position);
        }

        if (other.tag == "Minion" && !anim.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle"))
        {
            other.GetComponent<Enemy>().TakeDamage(15, transform.forward);
        }

    }

}
