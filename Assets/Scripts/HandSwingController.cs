//using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class HandSwingController : MonoBehaviour
{
    public float movementSpeed = 20.0f; // Adjust this value to control movement speed
    public GameObject camera; // The player actually
    public GameObject left_hand; // The left hand's transform
    public GameObject right_hand; // The right hand's transform

    public Transform temp; // for testing

    public Transform cameraTransform; // The camera's transform
    public Transform leftHandTransform; // The left hand's transform
    public Transform rightHandTransform; // The right hand's transform

    private Vector3 lastLeftHandPosition; // The position of the left hand in the previous frame
    private Vector3 lastRightHandPosition; // The position of the right hand in the previous frame
    public float handStillThreshold = 1f;

    public GameObject right_hand_prefab;
    public GameObject left_hand_prefab;

    public OVRInput.Controller r_controller;
    public OVRInput.Controller l_controller;
    //raycast used to check if the player is on the ground
    private RaycastHit groundHit;
    //distance allowance for the raycast
    public float groundDistance = 2f;
    //vectors for hand velocity
    private Vector3 leftHandVelocity;
    private Vector3 rightHandVelocity;
    //vectors for hand velocity magnitude
    private float leftHandMagnitude;
    private float rightHandMagnitude;
    void Start()
    {
        cameraTransform = camera.transform;
        leftHandTransform = left_hand.transform;
        rightHandTransform = right_hand.transform;

        lastLeftHandPosition = leftHandTransform.position; // Set the initial left hand position
        lastRightHandPosition = rightHandTransform.position; // Set the initial right hand position


        r_controller = right_hand_prefab.GetComponent<OVRControllerHelper>().m_controller;
        l_controller = left_hand_prefab.GetComponent<OVRControllerHelper>().m_controller;

    }

    void Update()
    {
    
    }

    void FixedUpdate()
    {
        // Calculate the velocity of each hand by subtracting the current position from the last position and dividing by the time delta
        leftHandVelocity = (leftHandTransform.localPosition - lastLeftHandPosition) / Time.deltaTime;
        rightHandVelocity = (rightHandTransform.localPosition - lastRightHandPosition) / Time.deltaTime;

        // Calculate the magnitude of each hand velocity to check if the hand is still
        leftHandMagnitude = leftHandVelocity.magnitude;
        rightHandMagnitude = rightHandVelocity.magnitude;

        Debug.Log("right hand trigger: " + OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger));
        //cast a ray down from the player's position
        //if the raycast hits the and is under, then the player is on the ground
        if (Physics.Raycast(cameraTransform.position, Vector3.down, out groundHit, groundDistance))
        {
            //log the distance to the ground
            Debug.Log("distance to ground: " + groundHit.distance);
            //if the player is on the ground, run the skateUpdate function
            if (groundHit.distance < groundDistance)
            {
                skateUpdate();
            }

        }

        
        
        /*
        if (camera.GetComponent<Rigidbody>().velocity < speed_limit)
        {
            SlowDown(temp.back, camera.GetComponent<Rigidbody>());
        }
        */
        lastLeftHandPosition = leftHandTransform.localPosition; // Update the last left hand position
        lastRightHandPosition = rightHandTransform.localPosition; // Update the last right hand position
        /*
        // Update player's rotation based on camera's rotation
        Vector3 cameraRotationEuler = temp.rotation.eulerAngles;
        cameraRotationEuler.x = 0f;
        cameraRotationEuler.z = 0f;
        Quaternion playerRotation = Quaternion.Euler(cameraRotationEuler);
        transform.rotation = playerRotation;
        */
    }
    //function that runs if the player is on the ground
    void skateUpdate()
    {
        // If either hand velocity is negative in the Z-axis (moving backward) and the hand is not still, move the camera forward by multiplying the camera's forward vector by the movement speed and the time delta
        if ((leftHandVelocity.z < 0 && leftHandMagnitude > handStillThreshold) || (rightHandVelocity.z < 0 && rightHandMagnitude > handStillThreshold))
        {
            //Using the move position
            //cameraTransform.position += cameraTransform.forward * movementSpeed * Time.deltaTime;
            //camera.GetComponent<Rigidbody>().MovePosition(cameraTransform.position += cameraTransform.forward * movementSpeed * Time.deltaTime);
            //camera.GetComponent<Rigidbody>().MovePosition(cameraTransform.position += temp.forward * movementSpeed * Time.deltaTime);

            //Using forces
            //camera.GetComponent<Rigidbody>().AddForce(temp.forward * movementSpeed, ForceMode.Impulse);
            //updated version that removes the y component of the force
            camera.GetComponent<Rigidbody>().AddForce(new Vector3(temp.forward.x, 0, temp.forward.z) * movementSpeed, ForceMode.Impulse);

            Debug.Log(temp.forward);
        }
        if (camera.GetComponent<Rigidbody>().velocity.magnitude > 0)
        {
            Debug.Log("got into first");
            
            if (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) > 0f || OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0f)
            {
                Debug.Log("got into breaking");
                
                //float magnitude = 3f;
                // Calculate the opposite force vector
                Vector3 oppositeForce = -camera.GetComponent<Rigidbody>().velocity.normalized * 3; // 3 is the magnitude that we can change

                camera.GetComponent<Rigidbody>().AddForce(oppositeForce, ForceMode.Impulse);
            }
            else
            {
                //Debug.Log("right hand trigger: " + OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger));

            }

            Debug.Log("right hand trigger: " + OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, r_controller));


        }
    }
    /*
    //Gets the forward vector of the rigidbody and applies a force vector in the opposite direction
    void SlowDown(Vector3 back_vector, RigidBody physics_moved_object)
    {
        //float braking_force 1.0f;
    }
    */
}
