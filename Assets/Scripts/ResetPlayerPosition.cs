using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour {
    

    //the starting position of the player
    private Vector3 startingPosition;
    //the player object
    public GameObject player;
    //the transform of the building spawn point
    public Transform buildingSpawnPoint;

    void Start() {
        // Record the starting position of the player
        startingPosition = player.transform.position;
        //if the starting position is below the ground, move it up
        if (startingPosition.y < 0) {
            startingPosition.y = 2f;
        }
    }

    void Update() {
        // Check if the reset key is pressed (the reset key is the left analog stick click)
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick)) {
            // Reset the player's position to the starting position
            player.transform.position = startingPosition;
            //reset the player's velocity
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        //check if the building spawn key is pressed (the building spawn key is the right analog stick click)
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick)) {
            //respawn the player at the building spawn point
            player.transform.position = buildingSpawnPoint.position;
            //reset the player's velocity
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
