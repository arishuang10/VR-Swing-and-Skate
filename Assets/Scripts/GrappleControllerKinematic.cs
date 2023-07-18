using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//The GrappleHook Controller
public class GrappleControllerKinematic : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    //the player object. The player object is a dynamic object which is affected by physics, and the only method of moving the player is by applying forces to it
    public GameObject player;
    public float maxDistance = 50f;
    public float minDistance = 2f;
    //grappleLayer 
    public LayerMask grappleLayer;
    //grapple pull force scalar -- when the player pulls away from the object, this force is applied to the player towards the object
    public float grapplePullForce;
    //max grapple pull force scalar -- the maximum force that can be applied to the player when pulling away from the object
    public float maxGrapplePullForce;
    //left trigger value
    public float leftTrigger;
    //right trigger value
    public float rightTrigger;
    //game object which has the line renderer for the grapple rope
    public GameObject leftLine;
    public GameObject rightLine;
    //visible line renderer for the grapple rope
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;
    //Variables for the line width and properties
    public float lineWidth = 0.1f;
    //the material for the line renderer. The material should be semi-transparent
    public Material lineMaterial;
    //enum used to determine which hand is grappling
    private enum GrapplingHand
    {
        Left,
        Right
    }
    //objects for each hand's grapple hook
    public GrappleHand leftGrapple;
    public GrappleHand rightGrapple;

    //physics variables set in the inspector
    public float mass = 1f;
    public float drag = 0f;
    public float angularDrag = 0.05f;
    //grapple reel-in speed
    public float grappleReelForce = 10f;
    //A scalar to multiply the grapple force's y-component by. This is used to cancel out the effects of gravity
    public float antiGravityForceMultiplier = 5f;
    //physics variables used in the script
    //the velocity of the player
    private Vector3 velocity;
    void Start()
    {
        //initialize the line renderers
        leftLineRenderer = leftLine.GetComponent<LineRenderer>();
        rightLineRenderer = rightLine.GetComponent<LineRenderer>();
        //edit the line renderer properties
        leftLineRenderer.startWidth = lineWidth;
        leftLineRenderer.endWidth = lineWidth;
        
        rightLineRenderer.startWidth = lineWidth;
        rightLineRenderer.endWidth = lineWidth;
        
        //initialize the grapple hook objects with the transform of the hand
        leftGrapple = new GrappleHand(leftHand.transform, leftLineRenderer, lineMaterial);
        rightGrapple = new GrappleHand(rightHand.transform, rightLineRenderer, lineMaterial);
    }
    //used for drawing
    void Update()
    {
        //draw the rope
        drawRay(GrapplingHand.Left);
        drawRay(GrapplingHand.Right);
    }
    //used for physics
    void FixedUpdate()
    {
        //set the trigger values
        leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        rightTrigger = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        // Check for left hand grapple if the player is not already grappling
        if (leftTrigger > 0 && !leftGrapple.grappling)
        {
            //cast the grapple ray and perform the grapple actions if the ray hits an object (all done in the castGrappleRay function)
            castGrappleRay(GrapplingHand.Left);

        }
        // If the player is already grappling and the left trigger is released, ungrapple the player
        else if (leftTrigger == 0 && leftGrapple.grappling)
        {
            //TODO: Ungrapple the player from the object
            //release the grapple
            releaseGrapple(GrapplingHand.Left);
            
        }
        //if the player is already grappling, move the player to the object and show the rope
        else if (leftGrapple.grappling && leftGrapple.hit.collider != null)
        {
            //run the updateGrapple function
            updateGrapple(GrapplingHand.Left);
        }

        // Check for right hand grapple if the player is not already grappling
        if (rightTrigger > 0 && !rightGrapple.grappling)
        {
            
            //cast the grapple ray and perform the grapple actions if the ray hits an object (all done in the castGrappleRay function)
            castGrappleRay(GrapplingHand.Right);
        }
        // If the player is already grappling and the right trigger is released, ungrapple the player
        else if (rightTrigger == 0 && rightGrapple.grappling)
        {
            //TODO: Ungrapple the player from the object
            //release the grapple
            releaseGrapple(GrapplingHand.Right);
        }
        //if the player is already grappling, move the player to the object and show the rope
        else if (rightGrapple.grappling && rightGrapple.hit.collider != null)
        {
            //run the updateGrapple function
            updateGrapple(GrapplingHand.Right);
        }
        //update the previous hand position (in the objects)
        leftGrapple.lastHandPosition = leftHand.transform.position;
        rightGrapple.lastHandPosition = rightHand.transform.position;
        
        //TODO: implement this for a physics-object player to get collisions working
        //temporary: update the player's position based on the velocity (this does not work with collisions)
        updatePosition();
    }
    //position update function -- update the position of the player based on the velocity
    //TODO: remove this when the player is a physics object
    private void updatePosition()
    {   
        //update the position of the player
        player.transform.position += velocity * Time.deltaTime;
    }
    //function which runs at the initiation of a grapple
    private void startGrapple(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //set the grapple position to the position of the object hit by the grapple hook
        //grappleHand.grapplePosition = grappleHand.grappleObject.transform.position;

        //set the grapple hook to grappling
        grappleHand.grappling = true;
        //set the last grapple object position to the position of the object hit by the grapple hook
        grappleHand.lastGrappleObjectPosition = grappleHand.grappleObject.transform.position;
        //set the last hand position to the current hand position
        grappleHand.lastHandPosition = (hand == GrapplingHand.Left ? leftHand.transform.position : rightHand.transform.position);
        //set the previous distance to the distance between the player and the hit point
        grappleHand.previousGrappleDistance = Vector3.Distance(grappleHand.lastHandPosition, grappleHand.hit.point);
        //check if the object hit by the grapple hook is a moving object. We know it is a moving object if it has the MovingObject tag
        if (grappleHand.grappleObject.tag == "MovingObject")
        {
            //set the grapple rope color to green
            grappleHand.grappleLine.material.color = Color.green;
            //set the grapple object moving to true
            grappleHand.grappleObjectMoving = true;
        }
        else{
            //set the grapple rope color to cyan
            grappleHand.grappleLine.material.color = Color.cyan;
        }
    }
    //function which releases the current grapple
    private void releaseGrapple(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //ungrapple the player
        grappleHand.grappling = false;
        //set the object hit by the grapple hook to null
        grappleHand.grappleObject = null;
        //set the grapple line color to red
        grappleHand.grappleLine.material.color = Color.red;
        //set the grapple hand moving object to false
        grappleHand.grappleObjectMoving = false;
        

    }
    //function which updates a current grapple
    private void updateGrapple(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //check if the object hit by the grapple hook is a moving object
        if (grappleHand.grappleObjectMoving)
        {
            //if the object moved, update the position of the object hit by the grapple hook
            //grappleHand.grapplePosition = grappleHand.grappleObject.transform.position;
            //transform the grapplePosition the same amount the object moved
            grappleHand.grapplePosition += grappleHand.grappleObject.transform.position - grappleHand.lastGrappleObjectPosition;
            //update the last grapple object position
            grappleHand.lastGrappleObjectPosition = grappleHand.grappleObject.transform.position;
            //change the color of the rope to green
            grappleHand.grappleLine.material.color = Color.green;
        }
        //get the distance between the hand and the object
        float grappleDistance = Vector3.Distance(grappleHand.handTransform.position, grappleHand.grapplePosition);
        //if the object is close enough but not too close, grapple the player to the object
        if (grappleDistance < maxDistance && grappleDistance > minDistance)
        {
            //check if the object hit by the grapple hook moved
            // if (grappleHand.grappleObject.transform.position != grappleHand.lastGrappleObjectPosition)
            // {
            //     //if the object moved, update the position of the object hit by the grapple hook
            //     grappleHand.grapplePosition = grappleHand.grappleObject.transform.position;
            // }
            //make sure that the player doesn't move away from the object
            GrappleStay(hand);
            //pull the player towards the object
            //GrapplePull(hand);
        }
        //if the object is too far away or too close, ungrapple the player
        else
        {
            //release the grapple
            releaseGrapple(hand);
        }
        //if this is the left hand and the player holds the x button, reel in the left grapple
        if (hand == GrapplingHand.Left && leftGrapple.grappling && OVRInput.Get(OVRInput.Button.Three))
        {
            //reel in the left grapple
            reelGrapple(GrapplingHand.Left);
        }
        //if this is the right hand and the player holds the a button, reel in the right grapple
        if (hand == GrapplingHand.Right && rightGrapple.grappling && OVRInput.Get(OVRInput.Button.One))
        {
            //reel in the right grapple
            reelGrapple(GrapplingHand.Right);
        }
        //update the previous distance
        grappleHand.previousGrappleDistance = grappleDistance;
    }
    //function which casts a ray from the hand and performs the grapple actions if the ray hits an object
    private void castGrappleRay(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        if (Physics.Raycast(grappleHand.handTransform.position, grappleHand.handTransform.forward, out grappleHand.hit, maxDistance, grappleLayer))
        {
            //get the correct hand transform
            Transform handTransform = grappleHand.handTransform;
            //get the distance between the hand and the object
            float grappleDistance = Vector3.Distance(handTransform.position, grappleHand.hit.point);

            //if the object is close enough, grapple the player to the object
            if (grappleDistance < maxDistance && grappleDistance > minDistance)
            {
                //get the position of the object hit for the specific hand in question
                grappleHand.grapplePosition = grappleHand.hit.point;
                //get the object hit by the grapple hook
                grappleHand.grappleObject = GetGrappleObject(grappleHand.hit);
                //start the grapple
                startGrapple(hand);
            }
        }
    }
    
    //function which gets the object hit by the grapple hook
    private GameObject GetGrappleObject(RaycastHit hit)
    {
        //if the object hit is a child of another object and the object is not tagged a MovingObject, get the parent object
        if (hit.collider.transform.parent != null && hit.collider.gameObject.tag != "MovingObject")
        {
            return hit.collider.transform.parent.gameObject;
        }
        //if the object hit is not a child of another object, return the object hit
        else
        {
            return hit.collider.gameObject;
        }
    }
    //grapple "stay" function that takes in the position of the hand and the object hit
    private void GrappleStay(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //get the direction of the grapple rope
        Vector3 grappleDirection = grappleHand.grapplePosition - grappleHand.handTransform.position;
        //get the distance between the hand and the object
        float grappleDistance = grappleDirection.magnitude;
        //get the direction of the grapple rope
        grappleDirection.Normalize();
        //get the velocity of the player
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;
        //get the velocity of the player in the direction of the grapple rope
        float playerVelocityInGrappleDirection = Vector3.Dot(playerVelocity, grappleDirection);
        //TODO: change this to make it smoother, such as by applying a force based on how far away the player is from the original grapple distance
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
    private void GrapplePull(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //get the direction of the grapple rope
        Vector3 grappleDirection = grappleHand.grapplePosition - grappleHand.handTransform.position;
        //get the distance between the hand and the object
        float grappleDistance = grappleDirection.magnitude;
        //get the direction of the grapple rope
        grappleDirection.Normalize();
        //compare the previous distance to the current distance
        float grappleDistanceDifference = grappleDistance - grappleHand.previousGrappleDistance;
        //if the grapple distance is increasing, pull the player towards the object
        if (grappleDistanceDifference > 0)
        {
            //get the grapple pull force multiplied by the difference in distance
            float realGrapplePullForce = grapplePullForce * grappleDistanceDifference;
            //if the grapple pull force is greater than the maximum grapple pull force, set the grapple pull force to the maximum grapple pull force
            if (realGrapplePullForce > maxGrapplePullForce)
            {
                realGrapplePullForce = maxGrapplePullForce;
            }
            //force the player towards the object
            applyImpulse(grappleDirection * realGrapplePullForce);
        }
    }
    //function which draws the visible ray between the hand and the object hit by the grapple hook. 
    //If there is no object hit, the ray is drawn to the maximum grapple distance with a red color.
    //If there is an object hit, the ray is drawn to the object with a cyan color.
    private void drawRay(GrapplingHand hand)
    {
        //set a reference to the GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //instead of using the grappleHand object, get the hand transform directly
        GameObject handObject = (hand == GrapplingHand.Left ? leftHand : rightHand);
        //if the grapple hook is grappling
        if (grappleHand.grappling)
        {
            //set the position of the first point of the ray to the position of the hand
            grappleHand.grappleLine.SetPosition(0, handObject.transform.position);
            //set the position of the second point of the ray to the position of the grapple hit
            grappleHand.grappleLine.SetPosition(1, grappleHand.grapplePosition);
            //set the color of the ray to cyan
            //grappleHand.grappleLine.material.color = Color.cyan;
        }
        //if the grapple hook is not grappling
        else
        {
            //set the position of the first point of the ray to the position of the hand
            grappleHand.grappleLine.SetPosition(0, handObject.transform.position);
            //get the distance between the hand and the object hit
            //grappleHand.grappleDistance = (grappleHand.grapplePosition - grappleHand.handTransform.position).magnitude;
            //TODO: set the position of the second point of the ray to the maximum grapple distance, or the distance between the hand and the object hit, whichever is smaller
            //grappleHand.grappleLine.SetPosition(1, grappleHand.handTransform.position + grappleHand.handTransform.forward * Mathf.Min(maxDistance, grappleHand.grappleDistance));
            //set the position of the second point of the ray to the maximum grapple distance
            grappleHand.grappleLine.SetPosition(1, handObject.transform.position + handObject.transform.forward * maxDistance);
            //set the color of the ray to red
            //grappleHand.grappleLine.startColor = Color.red;
            //grappleHand.grappleLine.endColor = Color.red;
            //use setLineColors instead of startColor and endColor because the latter are deprecated
            //grappleHand.grappleLine.material.color = Color.red;
        }
    }
    //function that reels in the grapple
    private void reelGrapple(GrapplingHand hand)
    {
        //reference the correct GrappleHand object
        GrappleHand grappleHand = (hand == GrapplingHand.Left ? leftGrapple : rightGrapple);
        //get the normalized direction vector of the grapple rope
        Vector3 grappleDirection = (grappleHand.grapplePosition - grappleHand.handTransform.position).normalized;
        //apply an impulse force to the player in the direction of the grapple rope
        applyImpulse(grappleDirection * grappleReelForce);
    }
    //functions which handle the movement physics, since the object being moved is kinematic and cannot be updated via Unity's physics engine
    //function which applies an impulse force to the player
    private void applyImpulse(Vector3 impulse)
    {
        //apply the impulse to the player by increasing the player's velocity by the acceleration of the impulse
        float acceleration = impulse.magnitude;
        //velocity += acceleration * impulse.normalized;
        player.GetComponent<Rigidbody>().velocity += acceleration * impulse.normalized;
    }
}
//class which holds the variables for a specific hand's grapple hook
// public class GrappleHand
// {
//     //variables for the grapple hook
//     //whether or not the grapple hook is currently grappling
//     public bool grappling;
//     //the raycast hit of the grapple hook
//     public RaycastHit hit;
//     //the position of the object hit by the grapple hook
//     public Vector3 grapplePosition;
//     //the object hit by the grapple hook
//     public GameObject grappleObject;
//     //the last position of the object hit by the grapple hook
//     public Vector3 lastGrappleObjectPosition;
//     //the last position of the hand
//     public Vector3 lastHandPosition;
//     //the previous distance between the hand and the object hit by the grapple hook
//     //the visible ray between the hand and the object hit by the grapple hook
//     public LineRenderer grappleLine;
//     //the distance between the hand and the object hit by the grapple ray
//     public float grappleDistance;

    

//     public Transform handTransform;
//     //constructor for the grapple hook which initializes the variables.
//     public GrappleHand(Transform handTransform, LineRenderer grappleLine, Material lineMaterial)
//     {
//         grappling = false;
//         this.grappleLine = grappleLine;
//         this.handTransform = handTransform;
//         lastHandPosition = this.handTransform.position;
//         //set grappleDistance to infinity
//         grappleDistance = Mathf.Infinity;
//         //set the line material to the material passed in
//         grappleLine.material = lineMaterial;
//         //set the color of the line to red
//         grappleLine.material.color = Color.red;
//     }
// }

