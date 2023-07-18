using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door_parent;
    public Boolean isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isOpen)
        {
            door_parent.transform.Rotate(0, -90, 0);
            isOpen = true;
        }
        
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (isOpen)
        {
            door_parent.transform.Rotate(0, 90, 0);
            isOpen = false;
        }
    }
    

}
