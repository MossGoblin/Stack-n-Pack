using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour
{
    // refs
    [SerializeField] private Conductor master;
    [SerializeField] private GameObject popup;
    [SerializeField] private TextMeshProUGUI popupText;

    private const string pauseText = "- GAME PAUSED -\nEnter : continue\nQ : Quit";
    private const string winText = "You got MAX!\nYou WIN!";
    private const string noZapText = "You are out of ZAP!";

    // TextMesh popupText;
    // Start is called before the first frame update
    void Start()
    {
        popup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (master.gamePaused)
        {
            popup.SetActive(true);
            popupText.text = pauseText;
            // popupText.GetComponent<TMPro.TextMeshProUGUI>().text = pauseText;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                popup.SetActive(false);
                master.gamePaused = false;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Quit Game");
                Application.Quit();
            }
        }   
    }

    public void PopUpLose()
    {
        popup.SetActive(true);
        popupText.text = noZapText;
        if (Input.anyKeyDown)
            {
                Debug.Log("End Game - Lose");
                Application.Quit();
            }
    }

    public void PopUpWin()
    {
        popup.SetActive(true);
        popupText.text = winText;
        if (Input.anyKeyDown)
            {
                Debug.Log("End Game - Win");
                Application.Quit();
            }
    }
}
