using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffCharge : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().TakeDamage(Player.instance.chargeDamage, transform.forward);
        }
    }
}
