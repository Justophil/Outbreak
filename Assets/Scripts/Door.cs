using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay (Collider other) {
        if (other.tag == "player") {
            if (Input.GetKey ("e")) {
               doorOpen();
            }
        }
    }
 
    void doorOpen()
    {
        // set values to trigger opening a door goes here       
    }
}
