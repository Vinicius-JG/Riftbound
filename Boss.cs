using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    static public Boss instance;
    Animator anim;
    public float timeToAttack;
    float currentTimeToNextAttack;
    public float speed;
    public Transform target;
    bool stopped;
    bool atkStopped;

    public GameObject skeleton;
    bool summoned;

    private void Start()
    {
        instance = this;
        GameController.instance.bossFightStart = true;
        anim = GetComponent<Animator>();
        currentTimeToNextAttack = timeToAttack;
        target = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (GameObject.Find("Game Controller").GetComponent<GameController>().bossFightStart)
        {
            // escolha de alvo
            if(GameObject.FindGameObjectsWithTag("Minion").Length <= 0)
            {
                target = GameObject.Find("Player").transform;
                summoned = false;
            }
            else
            {
                target = GameObject.FindGameObjectsWithTag("Minion")[0].transform;
                if (!summoned)
                {
                    Instantiate(skeleton, transform.position + (transform.right * 3), transform.rotation);
                    Instantiate(skeleton, transform.position - (transform.right * 3), transform.rotation);
                    Instantiate(skeleton, transform.position + (transform.forward * 3), transform.rotation);
                    Instantiate(skeleton, transform.position - (transform.forward * 3), transform.rotation);
                    summoned = true;
                }
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Boss_Damaged"))
            {
                currentTimeToNextAttack -= Time.deltaTime;
            }

            if (currentTimeToNextAttack <= 0)
            {
                anim.SetInteger("Attack", Random.Range(1, 5));
                currentTimeToNextAttack = timeToAttack;
            }
            else
            {
                anim.SetInteger("Attack", 0);
            }

            if (Vector3.Distance(transform.position, target.position) < 4)
            {
                stopped = true;
            }
            else if(atkStopped == false)
            {
                stopped = false;
            }

            if (!stopped && !atkStopped)
            {
                anim.SetBool("Walking", true);
                transform.LookAt(target);
                transform.position = Vector3.MoveTowards(transform.position, target.position - transform.forward * 2, speed * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Walking", false);
            }

        }
    }

    IEnumerator Stop()
    {
        atkStopped = true;
        yield return new WaitForSeconds(2f);
        atkStopped = false;
    }

}
