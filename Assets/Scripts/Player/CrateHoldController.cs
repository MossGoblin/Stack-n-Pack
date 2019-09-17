using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrateHoldController : MonoBehaviour
{

    [SerializeField] Sprite crateEmpty;
    [SerializeField] Sprite crateOne;
    [SerializeField] Sprite crateTwo;
    [SerializeField] Sprite crateThree;
    [SerializeField] Sprite crateFour;

    [SerializeField] GameObject player;

    Image cargoImage;


    // Start is called before the first frame update
    void Start()
    {
        cargoImage = GetComponent<Image>();
        cargoImage.sprite = crateThree;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject crateOnHold = player.GetComponent<PlayerController>().crateOnHold;
        if (crateOnHold == null)
        {
            cargoImage.sprite = crateEmpty;
        }
        else if(crateOnHold.name == "crate 01")
        {
            cargoImage.sprite = crateOne;
        }
        else if (crateOnHold.name == "crate 02")
        {
            cargoImage.sprite = crateTwo;
        }
        else if (crateOnHold.name == "crate 03")
        {
            cargoImage.sprite = crateThree;
        }
        else if (crateOnHold.name == "crate 04")
        {
            cargoImage.sprite = crateFour;
        }
    }
}
