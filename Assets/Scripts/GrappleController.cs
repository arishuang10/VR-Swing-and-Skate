using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    //the player object. The player object is a dynamic object which is affected by physics, and the only method of moving the player is by applying forces to it
    public GameObject player;
    public float maxDistance = 50f;
    public LayerMask grappleLayer;

    private bool leftGrappling = false;
    private bool rightGrappling = false;
    private RaycastHit leftHit;
    private RaycastHit rightHit;
    //the position of the object hit by the left grappling hook
    private Vector3 leftObjectPosition;
    //the position of the object hit by the right grappling hook
    private Vector3 rightObjectPosition;
    //grapple pull force scalar -- when the player pulls away from the object, this force is applied to the player towards the object
    public float grapplePullForce = 10f;
    //previous hand position
    private Vector3 lastLeftHandPosition;
    private Vector3 lastRightHandPosition;

    //enum used to determine which hand is grappling
    private enum GrapplingHand
    {
        Left,
        Right
    }
    void Start()
    {
        leftHandTransform = leftHand.transform;
        rightHandTransform = rightHand.transform;
    }
    void Update()
    {
        // Check for left hand grapple if the player is not already grappling
        if (Input.GetAxisRaw("Left Trigger") > 0 && !leftGrappling)
        {
            if (Physics.Raycast(leftHandTransform.position, leftHandTransform.forward, out leftHit, maxDistance, grappleLayer))
            {
                //get the position of the object hit
                leftObjectPosition = leftHit.point;
                //get the distance between the hand and the object
                float grappleDistance = Vector3.Distance(leftHandTransform.position, leftObjectPosition);
                //if the object is close enough, grapple the player to the object
                if (grappleDistance < maxDistance)
                {
                    leftGrappling = true;
                }
            }
        }
        // If the player is already grappling and the left trigger is released, ungrapple the player
        else if (Input.GetAxisRaw("Left Trigger") == 0 && leftGrappling)
        {
            leftGrappling = false;
            //TODO: Ungrapple the player from the object
            
        }
        //if the player is already grappling, move the player to the object and show the rope
        else if (leftGrappling && leftHit.collider != null)
        {
            //get the distance between the hand and the object
            float grappleDistance = Vector3.Distance(leftHandTransform.position, leftObjectPosition);
            //if the object is close enough, grapple the player to the object
            if (grappleDistance < maxDistance)
            {
                //make sure that the player doesn't move away from the object
                GrappleStay(leftHandTransform.position, leftObjectPosition);
                //pull the player towards the object
                GrapplePull(leftHandTransform.position, leftObjectPosition, GrapplingHand.Left);
            }
            //if the object is too far away, ungrapple the player
            else
            {
                leftGrappling = false;
            }
        }

        // Check for right hand grapple if the player is not already grappling
        if (Input.GetAxisRaw("Right Trigger") > 0 && !rightGrappling)
        {
            if (Physics.Raycast(rightHandTransform.position, rightHandTransform.forward, out rightHit, maxDistance, grappleLayer))
            {
                //get the position of the object hit
                rightObjectPosition = rightHit.point;
                //get the distance between the hand and the object
                float grappleDistance = Vector3.Distance(rightHandTransform.position, rightObjectPosition);
                //if the object is close enough, grapple the player to the object
                if (grappleDistance < maxDistance)
                {
                    rightGrappling = true;
                }
            }
        }
        // If the player is already grappling and the right trigger is released, ungrapple the player
        else if (Input.GetAxisRaw("Right Trigger") == 0 && rightGrappling)
        {
            rightGrappling = false;
            //TODO: Ungrapple the player from the object
        }
        //if the player is already grappling, move the player to the object and show the rope
        else if (rightGrappling && rightHit.collider != null)
        {
            //get the distance between the hand and the object
            float grappleDistance = Vector3.Distance(rightHandTransform.position, rightObjectPosition);
            //if the object is close enough, grapple the player to the object
            if (grappleDistance < maxDistance)
            {
                //make sure that the player doesn't move away from the object
                GrappleStay(rightHandTransform.position, rightObjectPosition);
                //pull the player towards the object
                GrapplePull(rightHandTransform.position, rightObjectPosition, GrapplingHand.Right);
            }
            //if the object is too far away, ungrapple the player
            else
            {
                rightGrappling = false;
            }
        }
        //update the previous hand position
        lastLeftHandPosition = leftHandTransform.position;
        lastRightHandPosition = rightHandTransform.position;
    }
    //grapple "stay" function that takes in the position of the hand and the object hit
    //this function cancels out forces in the opposite direction of the grapple rope
    void GrappleStay(Vector3 handPosition, Vector3 objectPosition)
    {
        //get the direction of the grapple rope
        Vector3 grappleDirection = objectPosition - handPosition;
        //get the distance between the hand and the object
        float grappleDistance = grappleDirection.magnitude;
        //get the direction of the grapple rope
        grappleDirection.Normalize();
        //get the velocity of the player
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        //get the velocity of the player in the direction of the grapple rope
        float playerVelocityInGrappleDirection = Vector3.Dot(playerVelocity, grappleDirection);
        //if the player is moving in the opposite direction of the grapple rope, cancel out the velocity in that direction
        if (playerVelocityInGrappleDirection < 0)
        {
            //get the velocity of the player in the opposite direction of the grapple rope
            Vector3 playerVelocityInOppositeDirection = playerVelocityInGrappleDirection * grappleDirection;
            //cancel out the velocity in the opposite direction of the grapple rope
            player.GetComponent<Rigidbody>().velocity -= playerVelocityInOppositeDirection;
        }
    }
    //grapple "pull" function that takes in the position of the hand and the object hit, and which hand is grappling
    //this function applies a force to the player in the direction of the grapple rope
    void GrapplePull(Vector3 handPosition, Vector3 objectPosition, GrapplingHand grapplingHand)
    {
        //get the previous hand position of the correct hand
        Vector3 lastHandPosition = (grapplingHand == GrapplingHand.Left) ? lastLeftHandPosition : lastRightHandPosition;
        //get the direction of the grapple rope
        Vector3 grappleDirection = objectPosition - handPosition;
        //get the distance between the hand and the object
        float grappleDistance = grappleDirection.magnitude;
        //get the direction of the grapple rope
        grappleDirection.Normalize();
        //get the difference in hand position
        Vector3 handPositionDifference = handPosition - lastHandPosition;
        //if the hand is moving away from the object, apply a force to the player in the direction of the grapple rope
        if (Vector3.Dot(handPositionDifference, grappleDirection) < 0)
        {
            //get the magnitude of the hand position difference along the vector of the grapple rope
            float handPositionDifferenceMagnitude = Vector3.Dot(handPositionDifference, grappleDirection);
            //get the force to apply to the player
            float grapplePullForceMagnitude = grapplePullForce * handPositionDifferenceMagnitude;
            //apply the force to the player
            player.GetComponent<Rigidbody>().AddForce(grapplePullForceMagnitude * grappleDirection, ForceMode.Impulse);
        }
    }
}
