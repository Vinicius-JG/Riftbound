using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public List<Enemy> enimiesInRange;
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if(transform.root.tag == "Player" || transform.root.tag == "Minion")
        {
            if (other.gameObject.tag == "Enemy")
                enimiesInRange.Add(other.gameObject.GetComponent<Enemy>());
        }

        if(transform.root.tag == "Enemy")
        {
            if (other.gameObject.tag == "Player")
                player = other.gameObject.GetComponent<Player>();

            if (other.gameObject.tag == "Minion")
                enimiesInRange.Add(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (transform.root.tag == "Player" || transform.root.tag == "Minion")
        {
            if (other.gameObject.tag == "Enemy")
                enimiesInRange.Remove(other.gameObject.GetComponent<Enemy>());
        }

        if (transform.root.tag == "Enemy")
        {
            if (other.gameObject.tag == "Player")
                player = null;
            if (other.gameObject.tag == "Minion")
                enimiesInRange.Remove(other.gameObject.GetComponent<Enemy>());
        }
    }

}
