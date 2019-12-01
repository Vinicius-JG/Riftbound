using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int typeIndex;
    public float life;
    Rigidbody rb;
    public Animator anim;
    Vector3 knockBackDir;
    NavMeshAgent agent;
    public Transform target;
    public bool canAttack;
    public float attackRate;
    public float seekRange;
    public float attackRange;
    public int damage;
    public Hitbox activeHitbox;
    public bool ally;
    public GameObject autoBolt;
    public GameObject shooter;
    Quaternion attackDir;
    public Renderer rend;
    public GameObject bossModel;
    public GameObject bossWeapons;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(AttackCooldown());
        if (typeIndex == 3)
            rend = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>();
    }

    private void Update()
    {
        if(typeIndex != 5)
        {
            //Travar moviementação durante animação de dano
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged"))
            {
                agent.isStopped = true;
                rb.velocity = knockBackDir * 250 * Time.deltaTime;
            }
            else //Movimentar fora da animação de dano
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Vulnerable"))
                {
                    if (typeIndex != 3)
                        rb.velocity = Vector3.zero;

                    if (target != null)
                    {
                        if (Vector3.Distance(transform.position, target.position) > attackRange && Vector3.Distance(transform.position, target.position) < seekRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
                        {
                            agent.isStopped = false;
                            SeekTarget();
                        }
                        else
                        {
                            if (canAttack && Vector3.Distance(transform.position, target.position) <= attackRange)
                            {
                                if (typeIndex == 1)
                                    Attack();

                                if (typeIndex == 2)
                                    CanAttackFalse();

                                if (typeIndex == 3)
                                {
                                    attackDir = transform.rotation;
                                    Attack();
                                }
                            }

                            agent.isStopped = true;
                        }
                    }
                }
                else
                {
                    agent.isStopped = true;
                }
            }

            //Aplica animação de andar
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_AttackToIdle"))
                agent.isStopped = true;
            else
                anim.SetBool("Walking", !agent.isStopped);

            //Olha para o alvo
            if (typeIndex != 3)
                transform.GetChild(0).LookAt(target);

            //Detectar hit durante ataque
            if (typeIndex == 1 && anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack") && activeHitbox.player != null)
            {
                if (target && target.tag == "Player") // se o alvo for o player
                {
                    if (!activeHitbox.player.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Damaged") && !activeHitbox.player.anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion"))
                        activeHitbox.player.TakeDamage(damage, transform.position);
                }
            }

            if (typeIndex == 1 && anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack") && activeHitbox.enimiesInRange.Count > 0)
            {
                if (target && target.tag == "Minion")
                {
                    if (activeHitbox.enimiesInRange[0] != null && !activeHitbox.enimiesInRange[0].anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged"))
                    {
                        activeHitbox.enimiesInRange[0].TakeDamage(damage, transform.forward);
                    }
                }
            }

            if (typeIndex == 3)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
                {
                    agent.isStopped = true;
                    transform.GetChild(0).LookAt(null);
                    transform.rotation = attackDir;
                    rb.velocity = transform.GetChild(0).forward * 750 * Time.deltaTime;
                }
                else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Vulnerable"))
                {
                    rb.velocity = Vector3.zero;
                    transform.GetChild(0).LookAt(target);
                    agent.isStopped = false;
                }
            }

            //Versão de esqueleto invocado pelo player

            if (ally)
            {
                transform.tag = "Minion";

                if (GameObject.FindGameObjectWithTag("Enemy") != null)
                    target = GameObject.FindGameObjectWithTag("Enemy").transform;

                agent.isStopped = false;
                SeekTarget();

                //Ataque do aliado

                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack") && activeHitbox.enimiesInRange.Count > 0)
                {
                    foreach (Enemy e in activeHitbox.enimiesInRange)
                    {
                        if (e != null && !e.anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged"))
                        {
                            e.TakeDamage(damage, transform.forward);
                        }
                    }
                }

                //Se destroi se o player mudar de arma
                if (!GameObject.Find("Staff"))
                    Destroy(gameObject);

            }
            else
            {
                //Decide se persegue o player o um minion dependendo da distância 
                if (GameObject.FindGameObjectsWithTag("Minion").Length > 0)
                {
                    foreach (GameObject m in GameObject.FindGameObjectsWithTag("Minion"))
                    {
                        if (Vector3.Distance(transform.position, m.transform.position) > Vector3.Distance(transform.position, Player.instance.transform.position))
                            target = Player.instance.transform;
                        else
                            target = m.transform;
                    }
                }
                else
                {
                    target = Player.instance.transform;
                }
            }
        }
    }

    void SeekTarget()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }

    public void TakeDamage(int damage, Vector3 attackDir)
    {
        if(typeIndex != 3 || anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack") || anim.GetCurrentAnimatorStateInfo(0).IsName("Vulnerable"))
        {
            if (typeIndex != 5 || !anim.GetCurrentAnimatorStateInfo(1).IsName("Boss_Damaged"))
            {
                life -= damage;

                if (life <= 0)
                {
                    if (typeIndex != 5)
                        StartCoroutine(Die());
                    else
                        StartCoroutine(BossDie());
                }

                knockBackDir = attackDir;

                anim.SetTrigger("Damaged");

                GameObject.Find("Cam").GetComponent<CameraBehaviour>().Shake();
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        canAttack = false;
        StartCoroutine(AttackCooldown());
    }

    void CanAttackFalse()
    {
        anim.SetTrigger("Attack");
        canAttack = false;
    }

    void Shot()
    {
        Instantiate(autoBolt, shooter.transform.position + transform.GetChild(0).forward, shooter.transform.rotation);
        attackRate = 2f;
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.25f);
        Player.instance.activeHitbox.enimiesInRange.Remove(this);
        GameController.instance.money += 10;
        Destroy(gameObject);
    }

    IEnumerator BossDie()
    {
        bossModel.SetActive(false);
        bossWeapons.SetActive(false);
        yield return new WaitForSeconds(3f);
        GameController.instance.EndGame();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (typeIndex == 3)
        {
            if (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Attack"))
            {
                if (collision.gameObject.tag == "Player")
                {
                    if (!collision.gameObject.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Damaged") && !collision.gameObject.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion"))
                        collision.gameObject.GetComponent<Player>().TakeDamage(damage, transform.position);
                }

                if (collision.gameObject.tag == "Minion")
                {
                    if (!collision.gameObject.GetComponent<Enemy>().anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged"))
                        collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, transform.forward);
                }
            }
        }

        if(typeIndex == 4)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (!collision.gameObject.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Damaged") && !collision.gameObject.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion"))
                    collision.gameObject.GetComponent<Player>().TakeDamage(damage, transform.position);
            }

            if (collision.gameObject.tag == "Minion")
            {
                if (!collision.gameObject.GetComponent<Enemy>().anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged"))
                    collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, transform.forward);
            }
        }
    }

    public void Vulnerable()
    {
        anim.SetTrigger("Vulnerable");
    }

}
