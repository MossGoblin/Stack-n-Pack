using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    // list of sprites
    [SerializeField] Sprite spriteEmpty;
    [SerializeField] Sprite spriteOne;
    [SerializeField] Sprite spriteTwo;
    [SerializeField] Sprite spriteThree;
    [SerializeField] Sprite spriteFour;
    [SerializeField] Sprite spriteFive;

    int activeCrateOnHold;
    SpriteRenderer activeSprite;

    // Start is called before the first frame update
    void Start()
    {
        activeSprite = GetComponent<SpriteRenderer>();
        activeSprite.sprite = spriteEmpty;
    }

    // Update is called once per frame
    void Update()
    {
        activeCrateOnHold = GetComponentInParent<PlayerController>().crateOnHold;
        switch (activeCrateOnHold)
        {
            case 1:
                activeSprite.sprite = spriteOne;
                break;
            case 2:
                activeSprite.sprite = spriteTwo;
                break;
            case 3:
                activeSprite.sprite = spriteThree;
                break;
            case 4:
                activeSprite.sprite = spriteFour;
                break;
            case 5:
                activeSprite.sprite = spriteFive;
                break;
            default:
                activeSprite.sprite = spriteEmpty;
                break;
        }
    }
}
