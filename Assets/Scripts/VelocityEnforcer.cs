using UnityEngine;
//This script enforces the maximum speed of the player, but only in the x and z directions and the upward y direction.
public class VelocityEnforcer : MonoBehaviour
{
    // Declare public variables here that can be set in the Inspector
    //max speed;
    public float maxSpeed = 10f;
    //the player rigidbody (not 2D)
    public Rigidbody rb;
    private Vector3 vel;
    private float velY;

    // This function is called every frame
    void LateUpdate()
    {
        //get the player's velocity
        vel = rb.velocity;
        //get the velocity in the y direction
        velY = vel.y;
        //check if the player is moving up at a speed greater than the max speed
        if (velY > maxSpeed)
        {
            //set the player's y velocity to the max speed
            velY = maxSpeed;
        }

        //get the velocity in the x and z directions
        vel = new Vector3(vel.x, 0, vel.z);
        //if the player's velocity in the non-y direction is greater than the max speed
        if (vel.magnitude > maxSpeed)
        {
            //set the player's velocity to the max speed
            vel = vel.normalized * maxSpeed;
        }
        //add the y velocity back to the player's velocity
        vel = new Vector3(vel.x, velY, vel.z);
        //set the player's velocity to the new velocity
        rb.velocity = vel;
    }
}
