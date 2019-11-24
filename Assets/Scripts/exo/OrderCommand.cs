using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderCommand : MonoBehaviour
{
    // elements
    public Transform[] typeDisplay;
    public Sprite[] images;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVisuals(int index)
    {
        int[] indices = new int[6];
        for (int count = 5; count >= 0; count--)
        {
            int newPartial = index%10;
            typeDisplay[count].GetComponent<Image>().sprite = images[newPartial];
            index = index/10;
        }
    }
}
