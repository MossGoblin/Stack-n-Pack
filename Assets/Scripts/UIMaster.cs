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

    public bool gameWin = false;
    public bool gameLoss = false;

    private const string pauseText = "- GAME PAUSED -\nEnter : continue\nQ : Quit";
    private const string winText = "You got MAX!\nYou WIN!\n( press any key to exit )";
    private const string noZapText = "You are out of ZAP!\n( press any key to exit )";

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
            if (gameWin)
            {
                popup.SetActive(true);
                popupText.text = winText;
                if (Input.anyKeyDown)
                {
                    Debug.Log("Quit Game");
                    Application.Quit();
                }
            }
            else if (gameLoss)
            {
                popup.SetActive(true);
                popupText.text = noZapText;
                if (Input.anyKeyDown)
                {
                    Debug.Log("Quit Game");
                    Application.Quit();
                }
            }
            else
            {
                popup.SetActive(true);
                popupText.text = pauseText;

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
    }
}
