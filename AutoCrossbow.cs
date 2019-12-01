using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCrossbow : MonoBehaviour
{
    public float range;
    public float shotRate;
    public GameObject bolt;
    public Transform target;

    private void Start()
    {
        InvokeRepeating("Shot", shotRate, shotRate);
    }

    private void Update()
    {
        //Se destroi se o player mudar de arma
        if (!GameObject.Find("Crossbow"))
            Destroy(gameObject);

        if (target != null)
            transform.LookAt(target);

    }

    void Shot()
    {
        print("Ok");
        if(GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
                if (Vector3.Distance(transform.position, e.transform.position) < range)
                    target = e.transform;

            if(target != null)
            {
                transform.LookAt(target);
                Instantiate(bolt, transform.position, transform.rotation);
            }
        }
    }

}
