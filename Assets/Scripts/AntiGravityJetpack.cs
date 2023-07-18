using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravityJetpack : MonoBehaviour {
    // The strength of the anti-gravity force
    public float antiGravityStrength = 1f;

    // The player object
    //public GameObject player;
    // The anti-gravity velocity
    private Vector3 antiGravityVelocity;
    // The player's rigidbody
    public Rigidbody rb;

    void Start() {
        // Calculate the anti-gravity velocity
        //antiGravityVelocity = -Physics.gravity * antiGravityStrength * Time.fixedDeltaTime;
        // Get the player's rigidbody
        //rb = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        //get the player input
        bool B_button = OVRInput.Get(OVRInput.Button.Two);
        // Get the player's rigidbody
        //Rigidbody rb = player.GetComponent<Rigidbody>();

        // Calculate the anti-gravity velocity
        //Vector3 antiGravityVelocity = -Physics.gravity * antiGravityStrength * Time.fixedDeltaTime;
        //check if the player is pressing the A button
        if (B_button) {
            // Add the anti-gravity velocity to the player's velocity
            //turn off the rigidbody's gravity
            rb.useGravity = false;
        }
        else {
            //turn on the rigidbody's gravity
            rb.useGravity = true;
        }

        // Add the anti-gravity velocity to the player's velocity
        //rb.velocity += antiGravityVelocity;
        //TODO: activate the jetpack particle effect
    }
}
