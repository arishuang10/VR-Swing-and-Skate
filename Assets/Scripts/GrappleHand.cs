using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHand
{
    //variables for the grapple hook
    //whether or not the grapple hook is currently grappling
    public bool grappling;
    //a boolean for whether the object grappled to is moving
    public bool grappleObjectMoving;
    //the raycast hit of the grapple hook
    public RaycastHit hit;
    //the position of the object hit by the grapple hook
    public Vector3 grapplePosition;
    //the object hit by the grapple hook
    public GameObject grappleObject;
    //the last position of the object hit by the grapple hook
    public Vector3 lastGrappleObjectPosition;
    //the last position of the hand
    public Vector3 lastHandPosition;
    //the previous distance between the hand and the object hit by the grapple hook
    //the visible ray between the hand and the object hit by the grapple hook
    public LineRenderer grappleLine;
    //the distance between the hand and the object hit by the grapple ray
    public float grappleDistance;
    //the previous distance between the hand and the object hit by the grapple hook
    public float previousGrappleDistance;

    

    public Transform handTransform;
    //constructor for the grapple hook which initializes the variables.
    public GrappleHand(Transform handTransform, LineRenderer grappleLine, Material lineMaterial)
    {
        grappling = false;
        grappleObjectMoving = false;
        this.grappleLine = grappleLine;
        this.handTransform = handTransform;
        lastHandPosition = this.handTransform.position;
        //set grappleDistance to infinity
        grappleDistance = Mathf.Infinity;
        //set the line material to the material passed in
        grappleLine.material = lineMaterial;
        //set the color of the line to red
        grappleLine.material.color = Color.red;
    }
}