using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static GameController instance;
    public bool bossFightStart;
    public int money;
    public int potions;
    public int keys;
    public GameObject prison;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            RestartGame();

        //Eventos scriptados

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && prison != null)
            StartCoroutine(BreakPrison());

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator BreakPrison()
    {
        yield return new WaitForSeconds(1.5f);

        prison.SetActive(false);
    }

    public void GetKey(GameObject key)
    {
        keys++;
        Destroy(key);
    }

    public void EndGame()
    {
        SceneManager.LoadScene("End");
    }

}
