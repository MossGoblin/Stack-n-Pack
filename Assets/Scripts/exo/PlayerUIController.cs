using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    // clamps sprite ref
    [SerializeField] Sprite clampSpriteClosed;
    [SerializeField] Sprite clampSpriteOpen;
    [SerializeField] Sprite crateImage;

    Image clampImage;
    Image contentImage;

    [SerializeField] PlayerController player;
    Transform playerClampsUI;
    Transform playerContentUI;

    // refs
    [SerializeField] CrateMaster crateMaster;
    
    // Start is called before the first frame update
    void Start()
    {
        playerContentUI = this.transform.GetChild(0);
        playerClampsUI = this.transform.GetChild(1);
        clampImage = playerClampsUI.GetComponent<Image>();
        clampImage.sprite = clampSpriteClosed;
        contentImage = playerContentUI.GetComponent<Image>();
        player = GameObject.Find("PlayerController").GetComponent<PlayerController>();
        UpdateContentImage();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.clampsOpen)
        {
            clampImage.sprite = clampSpriteOpen;
        }
        else
        {
            clampImage.sprite = clampSpriteClosed;

        }

        UpdateContentImage();
    }

    private void UpdateContentImage()
    {
        if (player.Content > 0)
        {
            contentImage.enabled = true;
            contentImage.color = crateMaster.colorPool[player.Content - 1];
        }
        else
        {
            contentImage.enabled = false;
        }
    }
}
