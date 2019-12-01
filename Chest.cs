using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public EnemySpawner[] spawnerToTrigger;

    private void Start()
    {
        if (spawnerToTrigger.Length > 0)
            foreach (EnemySpawner s in spawnerToTrigger)
                s.triggered = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            if (Input.GetKeyDown("f"))
            {
                Open();
            }
        }
    }

    void Open()
    {
        if (spawnerToTrigger.Length > 0)
            foreach (EnemySpawner s in spawnerToTrigger)
                s.Spawn();

        GameController.instance.money += 50;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

}
