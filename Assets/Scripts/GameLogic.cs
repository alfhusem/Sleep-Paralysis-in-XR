using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private bool toggle = true;
    private int v = 0;

    void Start() {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Check if the button is pressed
        if (OVRInput.GetDown(OVRInput.Button.Two)) 
        {
            v++;
            transform.GetChild(0).gameObject.SetActive(toggle);
            toggle = toggle ? false : true;
        }
        if (v > 3) {
            Destroy(gameObject);
        }
    }

    
}
