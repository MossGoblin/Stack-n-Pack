using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMaster : MonoBehaviour
{
    // refs
    [SerializeField] private Conductor master;
    [SerializeField] private GameObject popupText;
    [SerializeField] private TextMeshPro popupTextElement;

    // TextMesh popupText;
    // Start is called before the first frame update
    void Start()
    {
        popupText.SetActive(false);
        // string popupText = popupTextElement.GetComponent<TMPro.TextMeshProUGUI>().text;
    }

    // Update is called once per frame
    void Update()
    {

        if (master.gamePaused)
        {
            popupText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                popupText.SetActive(false);
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
