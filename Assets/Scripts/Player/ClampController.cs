using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClampController : MonoBehaviour
{
    // clamps sprite ref
    [SerializeField] Sprite clampSpriteClosed;
    [SerializeField] Sprite clampSpriteOpen;


    Image clampImage;

    // player ref
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        clampImage = GetComponent<Image>();
        clampImage.sprite = clampSpriteClosed;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().releaseFlag)
        {
            clampImage.sprite = clampSpriteOpen;
        }
        else
        {
            clampImage.sprite = clampSpriteClosed;

        }
    }
}
