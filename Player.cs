using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static public Player instance;
    public float normalSpeed;
    public float speed;
    public float evadeSpeed;
    public bool evading;
    public float evadeCost;
    bool canEvade = true;
    public float evadeCooldownTime;
    public bool resting;
    public float staminaRecoverRate;
    public float restCooldownTime;
    Vector3 evadeDir;
    Vector2 movement;
    Rigidbody rb;
    public Animator anim;
    public AnimatorOverrideController swordAnimator;
    public AnimatorOverrideController crossbowAnimator;
    public AnimatorOverrideController staffAnimator;

    public float life;
    public float stamina;
    public float maxLife;
    public float maxStamina;

    public int normalDamage;
    public int chargeDamage;
    public int specialDamage;

    public float normalCost;
    public float chargeCost;
    public float specialCost;

    public GameObject staffCharge;
    public GameObject autoCrossbow;

    public float comboCoolDownTime;
    bool canAttack = true;
    public Hitbox activeHitbox;

    float holdTime;

    public GameObject activeTrail;

    public int weaponSelected;
    public List<GameObject> weapons;

    public GameObject bolt;
    public Transform boltShooter;

    public GameObject missile;
    public Transform staffShooter;

    public GameObject shield;

    public GameObject ally;

    public GameObject damageScreenFX;

    Coroutine lastRoutine = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        instance = this.GetComponent<Player>();
        maxLife = 100;
        maxStamina = 100;
        speed = normalSpeed;
        UpdateWeapon();
    }

    void Update()
    {

        //Evasão

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Die"))
        {
            if (Input.GetButtonDown("Evade") && canEvade && stamina >= evadeCost)
                Evade();

            evading = anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion");

            if (evading && movement != Vector2.zero)
                transform.forward = evadeDir;

            if (resting)
            {
                if (stamina < maxStamina)
                    stamina += staminaRecoverRate * Time.deltaTime;
                else
                    stamina = maxStamina;
            }


            //Input de movimentação e direção
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            //Input de ataque
            if (Input.GetButtonUp("Attack") && canAttack && stamina >= normalCost && holdTime < 0.25f)
            {
                anim.SetTrigger("Attack");
                DecreaseStamina(normalCost);
            }

            //Ataque segurado
            if (Input.GetButton("Attack") && canAttack && stamina >= normalCost * 2)
            {
                holdTime += Time.deltaTime;
                resting = false;

                if (holdTime > 0.25f)
                {
                    anim.SetBool("Charging", true);
                }
            }

            if (Input.GetButtonUp("Attack"))
            {
                anim.SetBool("Charging", false);

                if (holdTime > 0.25f)
                    DecreaseStamina(chargeCost);

                holdTime = 0;
            }

            //Ataques especiais
            if (weapons[weaponSelected].name == "Sword" && stamina > 0)
                anim.SetBool("Defending", Input.GetButton("Special"));
            else
                anim.SetBool("Defending", false);

            if (weapons[weaponSelected].name == "Staff" && Input.GetButtonDown("Special") && stamina > specialCost && GameObject.FindGameObjectsWithTag("Minion").Length < 2)
                StaffSpecial();

            if (weapons[weaponSelected].name == "Crossbow" && Input.GetButtonDown("Special") && stamina > specialCost && GameObject.FindGameObjectsWithTag("Auto Crossbow").Length < 1)
                CrossbowSpecial();

            //Dano durante animação da espada
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_3"))
                SwordNormalAttack();

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeRelease"))
                SwordChargeAttack();

            //Ligar e desligar efeitos de ataque
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_3") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeRelease"))
                activeTrail.SetActive(true);
            else
                activeTrail.SetActive(false);

            LookDirection();

            if (movement.x != 0 || movement.y != 0)
            {
                anim.SetBool("Walking", true);
            }
            else
            {
                anim.SetBool("Walking", false);
            }

            //Trocar armas

            if (Input.GetKeyDown("q"))
            {
                PreviousWeapon();
            }

            if (Input.GetKeyDown("e"))
            {
                NextWeapon();
            }

            //Efeito de dano na tela

            damageScreenFX.SetActive(anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Damaged"));

            //Poção
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (GameController.instance.potions > 0)
                {
                    anim.SetTrigger("Drink");
                    GameController.instance.potions--;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!evading)
            Move();
        else
            rb.velocity = transform.forward * evadeSpeed * Time.fixedDeltaTime;
    }

    private void Move()
    {
        if (anim.GetBool("Charging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Defending"))
            speed = normalSpeed / 4;
        else
            speed = normalSpeed;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk") || anim.GetBool("Charging") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_ChargeRelease") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Defending"))
            rb.velocity = new Vector3(movement.x * speed * Time.fixedDeltaTime, 0, movement.y * speed * Time.fixedDeltaTime);
        else
            rb.velocity = Vector3.zero;
    }

    private void LookDirection()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Walk"))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(cameraRay, out hit))
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }

    void DecreaseStamina(float amount)
    {
        stamina -= amount;
        resting = false;

        if (stamina < 0)
            stamina = 0;

        if(lastRoutine != null)
            StopCoroutine(lastRoutine);

        lastRoutine = StartCoroutine(RestCooldown());
    }

    IEnumerator RestCooldown()
    {
        yield return new WaitForSeconds(restCooldownTime);

        resting = true;
    }

    void Evade()
    {
        anim.SetTrigger("Evade");

        evadeDir = new Vector3(movement.x, 0, movement.y).normalized;

        DecreaseStamina(evadeCost);

        canEvade = false;
        StartCoroutine(EvadeCooldown());
    }

    IEnumerator EvadeCooldown()
    {
        yield return new WaitForSeconds(evadeCooldownTime);

        canEvade = true;
    }

    void ComboEnd()
    {
        canAttack = false;
        StartCoroutine(ComboCooldown());
    }

    IEnumerator ComboCooldown()
    {
        yield return new WaitForSeconds(comboCoolDownTime);

        canAttack = true;
    }

    void SwordNormalAttack()
    {
        if (activeHitbox.enimiesInRange.Count > 0)
        {
            foreach (Enemy e in activeHitbox.enimiesInRange)
            {
                if (!e.anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_Damaged") && !e.anim.GetCurrentAnimatorStateInfo(0).IsName("Boss_Damaged"))
                {
                    e.TakeDamage(normalDamage, transform.forward);
                }
            }
        }
    }

    void SwordChargeAttack()
    {
        if (activeHitbox.enimiesInRange.Count > 0)
        {
            foreach (Enemy e in activeHitbox.enimiesInRange)
            {
                e.TakeDamage(chargeDamage, transform.forward);
            }
        }
    }

    void StaffShot()
    {
        Instantiate(missile, staffShooter.position, staffShooter.rotation);
    }

    void StaffSpecial()
    {
        if (GameObject.FindGameObjectsWithTag("Minion").Length == 0)
            Instantiate(ally, new Vector3(transform.position.x, transform.position.y, transform.position.z) - transform.right * 2, transform.rotation);

        Instantiate(ally, new Vector3(transform.position.x, transform.position.y, transform.position.z) + transform.right * 2, transform.rotation);

        DecreaseStamina(specialCost);
    }

    void StaffCharge()
    {
        staffCharge.SetActive(true);
    }

    void StaffChargeOff()
    {
        staffCharge.SetActive(false);
    }

    void CrossbowShot()
    {
        if(weapons[weaponSelected].GetComponentInChildren<Bolt>() != null)
            weapons[weaponSelected].GetComponentInChildren<Bolt>().Launch();
    }

    void CrossbowReload()
    {
        Instantiate(bolt, boltShooter.position, boltShooter.rotation);
    }

    void CrossbowSpecial()
    {
        Instantiate(autoCrossbow, boltShooter.position + (transform.forward*1.5f), boltShooter.rotation);
        DecreaseStamina(specialCost);
    }

    void NextWeapon()
    {
        if (weaponSelected < weapons.Count-1)
            weaponSelected++;
        else
            weaponSelected = 0;

        UpdateWeapon();
    }

    void PreviousWeapon()
    {
        if (weaponSelected > 0)
            weaponSelected--;
        else
            weaponSelected = weapons.Count-1;

        UpdateWeapon();
    }

    void UpdateWeapon()
    {
        foreach(GameObject w in weapons)
        {
            if(w != weapons[weaponSelected])
                w.SetActive(false);
            else
                w.SetActive(true);

            if (weapons[weaponSelected].name == "Sword")
            {
                anim.runtimeAnimatorController = swordAnimator;
                normalDamage = 25;
                chargeDamage = 100;
                specialDamage = 0;
                comboCoolDownTime = 0.25f;
                normalCost = 10;
                chargeCost = 75;
                specialCost = 0;
                activeHitbox = weapons[weaponSelected].GetComponentInChildren<Hitbox>();
                shield.SetActive(true);
            }
            
            if (weapons[weaponSelected].name == "Crossbow")
            {
                anim.runtimeAnimatorController = crossbowAnimator;
                normalDamage = 25;
                chargeDamage = 0;
                specialDamage = 0;
                comboCoolDownTime = 0.25f;
                normalCost = 15;
                chargeCost = 0;
                specialCost = 0;
                shield.SetActive(false);
            }

            if (weapons[weaponSelected].name == "Staff")
            {
                anim.runtimeAnimatorController = staffAnimator;
                normalDamage = 25;
                chargeDamage = 50;
                specialDamage = 0;
                comboCoolDownTime = 0.5f;
                normalCost = 25;
                chargeCost = 50;
                specialCost = 50;
                shield.SetActive(false);
            }
        }
    }

    void Heal()
    {
        life += 50;

        if(life > maxLife)
        {
            life = maxLife;
        }
    }

    public void TakeDamage(int damage, Vector3 enemyDir)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Die"))
        {
            if (!anim.GetBool("Defending") || stamina < damage)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Player_Evasion"))
                {
                    life -= damage;
                    GameObject.Find("Cam").GetComponent<CameraBehaviour>().Shake();
                    transform.LookAt(enemyDir);
                    anim.SetTrigger("Damaged");
                }
            }
            else
            {
                DecreaseStamina(damage);
                transform.LookAt(enemyDir);
            }

            if (life <= 0)
                StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(3f);

        GameController.instance.RestartGame();
    }

}
