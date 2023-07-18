using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public float moveSpeed = 0.1f;

    private Vector3 previousLeftHandPosition;
    private Vector3 previousRightHandPosition;
    private Vector3 initialCameraPosition;

    void Start()
    {
        previousLeftHandPosition = leftHand.transform.position;
        previousRightHandPosition = rightHand.transform.position;
        initialCameraPosition = transform.position;
    }

    void Update()
    {
        Vector3 leftHandMovement = leftHand.transform.position - previousLeftHandPosition;
        //Vector3 rightHandMovement = rightHand.transform.position - previousRightHandPosition;

        Vector3 cameraPosition = transform.position;
        cameraPosition -= leftHandMovement * moveSpeed;
        //cameraPosition -= rightHandMovement * moveSpeed;

        if (cameraPosition.z < initialCameraPosition.z)
        {
            // If it is, set the camera position to its initial position
            cameraPosition.z = initialCameraPosition.z;
        }
        else
        {
            //update and advance the new z axis
            initialCameraPosition.z = cameraPosition.z;
        }
        transform.position = cameraPosition;
        
        previousLeftHandPosition = leftHand.transform.position;
        //previousRightHandPosition= rightHand.transform.position;



    }

    void MoveForward()
    {

    }

    void MoveRight()
    {

    }

    void MoveLeft()
    {

    }
}
