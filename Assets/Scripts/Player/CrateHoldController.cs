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
    [SerializeField] Sprite crateFive;

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
        int crateOnHold = player.GetComponent<PlayerController>().crateOnHold;
        if (crateOnHold == 0)
        {
            cargoImage.sprite = crateEmpty;
        }
        else if(crateOnHold == 1)
        {
            cargoImage.sprite = crateOne;
        }
        else if (crateOnHold ==  2)
        {
            cargoImage.sprite = crateTwo;
        }
        else if (crateOnHold == 3)
        {
            cargoImage.sprite = crateThree;
        }
        else if (crateOnHold == 4)
        {
            cargoImage.sprite = crateFour;
        }
        else if (crateOnHold == 5)
        {
            cargoImage.sprite = crateFive;
        }
    }
}
