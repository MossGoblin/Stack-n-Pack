using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderVisuals : MonoBehaviour
{
    // refs
    public Transform sampleOrder;

    // Start is called before the first frame update
    void Start()
    {
            sampleOrder.GetComponent<OrderCommand>().UpdateVisuals(10210);
    }

    // Update is called once per frame
    void Update()
    {
        // GetTempInput();
    }

    private void GetTempInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sampleOrder.GetComponent<OrderCommand>().UpdateVisuals(10210);
        }
    }
}
