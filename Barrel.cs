using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public EnemySpawner[] spawnerToTrigger;

    private void Start()
    {
        if (spawnerToTrigger.Length > 0)
            foreach(EnemySpawner s in spawnerToTrigger)
                s.triggered = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HitBox")
        {
            if (GameObject.Find("Player").GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_1") || GameObject.Find("Player").GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_2") || GameObject.Find("Player").GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_3") || GameObject.Find("Player").GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeRelease"))
            {
                if(spawnerToTrigger.Length > 0)
                    foreach (EnemySpawner s in spawnerToTrigger)
                        s.Spawn();

                GameController.instance.money += 1;
                Destroy(gameObject);
            }
        }
    }
}
