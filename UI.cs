using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;
    public Image lifeBar;
    public Image bossLifeBar;
    public Image staminaBar;

    public GameObject dialogueBox;
    public Text dialogueTxt;

    public Text goldTxt;
    public Text potionTxt;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {

        if(GameController.instance.bossFightStart)
        {
            if(Boss.instance != null)
                bossLifeBar.fillAmount = Boss.instance.gameObject.GetComponent<Enemy>().life / 1800;

            bossLifeBar.enabled = true;
        }
        else
        {
            bossLifeBar.enabled = false;
        }

        lifeBar.fillAmount = Player.instance.life / Player.instance.maxLife;
        staminaBar.fillAmount = Player.instance.stamina / Player.instance.maxStamina;
        goldTxt.text = "Gold " + GameController.instance.money;
        potionTxt.text = "Potions " + GameController.instance.potions;
    }
}
