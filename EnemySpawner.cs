using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool spawned;
    public bool triggered;
    public int typeToSpawn;
    public GameObject[] enemies;
    public float distance = 10;

    private void Update()
    {
        if(Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < distance && !triggered)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        if (!spawned)
        {
            Instantiate(enemies[typeToSpawn], transform.position, transform.rotation);
            spawned = true;
        }
    }
}
