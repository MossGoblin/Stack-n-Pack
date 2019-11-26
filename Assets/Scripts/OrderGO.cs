using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderGO : MonoBehaviour
{
    // elements
    public Transform[] typeDisplay;
    public Sprite[] images;

    public Transform[] matchDisplay;
    public Sprite[] matchDigit;

    public Order orderData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisuals(orderData.ContentIndex);
    }

    public void UpdateVisuals(int[] index)
    {
        // display content
        for (int count = 0; count < 6; count++)
        {
            typeDisplay[count].GetComponent<Image>().sprite = images[index[count]];
        }

    }
}
