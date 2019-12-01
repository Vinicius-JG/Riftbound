using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !activated)
        {
            other.GetComponent<Player>().TakeDamage(25, transform.position);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            activated = true;
        }
    }
}
