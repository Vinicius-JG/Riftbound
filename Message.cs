using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public string messageTxt;
    public GameObject itemToBuy;
    public bool potion;
    public int price;
    public bool bought;
    public GameObject model;

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            UI.instance.dialogueBox.SetActive(true);
            UI.instance.dialogueTxt.text = messageTxt;
        }

        if(itemToBuy != null && !bought)
        {
            UI.instance.dialogueTxt.text = UI.instance.dialogueTxt.text + " (Price: " + price + ", Press 'F' to Buy)";

            if (Input.GetKeyDown("f") && GameController.instance.money >= price)
            {
                Player.instance.weapons.Add(itemToBuy);
                GameController.instance.money -= price;
                Destroy(model);
                bought = true;
            }
        }

        if (potion)
        {
            UI.instance.dialogueTxt.text = UI.instance.dialogueTxt.text + " (Price: " + price + ", Press 'F' to Buy)";

            if (Input.GetKeyDown("f") && GameController.instance.money >= price)
            {
                GameController.instance.potions++;
                GameController.instance.money -= price;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            UI.instance.dialogueTxt.text = "";
            UI.instance.dialogueBox.SetActive(false);
        }
    }

}
