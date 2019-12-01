using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Transform nearestEnemy;
    public int keysToOpen;
    public bool triggered;
    public GameObject wall;
    public bool bossDoor;

    private void Update()
    {
        foreach(GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(nearestEnemy == null || Vector3.Distance(transform.position, e.transform.position) < Vector3.Distance(transform.position, nearestEnemy.position))
            {
                nearestEnemy = e.transform;
            }
        }

        if(GameController.instance.keys >= keysToOpen && !triggered && !GameController.instance.bossFightStart)
        {
            if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 5 || nearestEnemy != null && Vector3.Distance(transform.position, nearestEnemy.position) < 5)
                GetComponent<Animator>().SetBool("Opened", true);
            else
                GetComponent<Animator>().SetBool("Opened", false);
        }

        if (triggered)
        {
            if(GameObject.FindGameObjectsWithTag("Enemy").Length <= 0 && GameController.instance.keys >= 1)
            {
                GetComponent<Animator>().SetBool("Opened", true);
                wall.SetActive(false);
            }
            else
            {
                GetComponent<Animator>().SetBool("Opened", false);
            }
        }

        if (bossDoor && GameController.instance.bossFightStart)
        {
            GetComponent<Animator>().SetBool("Opened", false);
        }

    }
}
