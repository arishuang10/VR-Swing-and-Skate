using UnityEngine;

public class LedgeClimber : MonoBehaviour {
    // The maximum climb angle in degrees
    public float climbAngle = 30f;

    // The maximum speed boost ceiling for the player to be boosted
    public float speedBoostCeiling = 2f;

    // The distance to cast the raycasts
    public float raycastDistance = 2f;
    //the distance for angle raycasts (use trig to calculate)
    private float angleRaycastDistance;

    // The layers to check for ledges
    public LayerMask ledgeLayers;

    // The player's rigidbody
    public Rigidbody rb;
    //The angles to check
    private Vector3[] angles = new Vector3[4];
    //the upward angles to check
    private Vector3[] upAngles = new Vector3[4];
    //the downward angles to check
    private Vector3[] downAngles = new Vector3[4];
    // a boolean array to check if the raycast hit the ledge
    private bool[] hitLedge = new bool[12];
    //a boolean for the ground raycast
    private bool hitGround;

    void Start() {
        //calculate the angle raycast distance
        //the raycast distance is the hypotenuse of a right triangle with the climbAngle as the angle and the raycastDistance as the opposite side
        angleRaycastDistance = 2 * raycastDistance / Mathf.Sin(climbAngle * Mathf.Deg2Rad);
        //get the angles to check
        //the first angle is forward
        angles[0] = transform.forward;
        //the second is right
        angles[1] = transform.right;
        //the third is back
        angles[2] = -transform.forward;
        //the fourth is left
        angles[3] = -transform.right;
        //get the upward angles to check
        //the first angle is forward climbAngle degrees up
        upAngles[0] = Quaternion.AngleAxis(climbAngle, transform.right) * transform.forward;
        //the second is right climbAngle degrees up
        upAngles[1] = Quaternion.AngleAxis(climbAngle, transform.up) * transform.right;
        //the third is back climbAngle degrees up
        upAngles[2] = Quaternion.AngleAxis(-climbAngle, transform.right) * -transform.forward;
        //the fourth is left climbAngle degrees up
        upAngles[3] = Quaternion.AngleAxis(-climbAngle, transform.up) * -transform.right;
        //get the downward angles to check
        //the first angle is forward climbAngle degrees down
        downAngles[0] = Quaternion.AngleAxis(-climbAngle, transform.right) * transform.forward;
        //the second is right climbAngle degrees down
        downAngles[1] = Quaternion.AngleAxis(-climbAngle, transform.up) * transform.right;
        //the third is back climbAngle degrees down
        downAngles[2] = Quaternion.AngleAxis(climbAngle, transform.right) * -transform.forward;
        //the fourth is left climbAngle degrees down
        downAngles[3] = Quaternion.AngleAxis(climbAngle, transform.up) * -transform.right;
    }

    void FixedUpdate() {
        //check if the player is on the ground
        hitGround = checkGrounded();
        //raycast in all 8 directions and check if the raycast hit the ledge. If it does, set the corresponding boolean to true
        for (int i = 0; i < 4; i++) {
            hitLedge[i] = CheckLedge(angles[i]);
            hitLedge[i + 4] = CheckLedgeAngled(upAngles[i]);
            hitLedge[i + 8] = CheckLedgeAngled(downAngles[i]);
            //if the first or third raycast hit, but the second didn't, 
            //then the player should be boosted up (if they aren't on the ground)
            if (!hitGround && (hitLedge[i] || hitLedge[i+8]) && !hitLedge[i + 4]) {
                BoostUp();
            }
            //if the first raycast didn't hit, but the third did and the second didn't, then the player should be boosted
            //in the direction of the first raycast
            if (!hitGround && !hitLedge[i] && hitLedge[i + 8] && !hitLedge[i + 4]) {
                //if i = 0, then the first raycast was forward
                if (i == 0) {
                    BoostForward();
                }
                //if i = 1, then the first raycast was right
                else if (i == 1) {
                    BoostRight();
                }
                //if i = 2, then the first raycast was back
                else if (i == 2) {
                    BoostBack();
                }
                //if i = 3, then the first raycast was left
                else if (i == 3) {
                    BoostLeft();
                }
            }
        }
        //debugging: if the raycast hit the ledge, draw a green line
        for (int i = 0; i < 12; i++) {
            if (hitLedge[i]) {
                //if in the first 4, draw a green line
                if (i < 4) {
                    Debug.DrawRay(transform.position, angles[i] * raycastDistance, Color.green);
                }
                //if in the second 4, draw a green line
                else if (i < 8) {
                    Debug.DrawRay(transform.position, upAngles[i - 4] * raycastDistance, Color.green);
                }
                //if in the third 4, draw a green line
                else if (i < 12) {
                    Debug.DrawRay(transform.position, downAngles[i - 8] * raycastDistance, Color.green);
                }
            }
        }
    }
    bool CheckLedge(Vector3 direction) {
        // Perform a raycast in the given direction
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, direction, out hitInfo, raycastDistance, ledgeLayers);

        // Draw a debug line to show the raycast
        Debug.DrawRay(transform.position, direction * raycastDistance, hit ? Color.green : Color.red);

        return hit;
    }
    //version of the above function for the angled raycasts
    bool CheckLedgeAngled(Vector3 direction) {
        // Perform a raycast in the given direction
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, direction, out hitInfo, angleRaycastDistance, ledgeLayers);

        // Draw a debug line to show the raycast
        Debug.DrawRay(transform.position, direction * angleRaycastDistance, hit ? Color.green : Color.red);

        return hit;
    }
    //function that checks if the player is on the ground using a raycast
    bool checkGrounded() {
        // Perform a raycast in the given direction
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, -transform.up, out hitInfo, raycastDistance, ledgeLayers);

        // Draw a debug line to show the raycast
        Debug.DrawRay(transform.position, -transform.up * raycastDistance, hit ? Color.green : Color.red);

        return hit;
    }
    //function that boosts the player up (not based on controls)
    void BoostUp()
    {
        //get the player's velocity in the y direction
        float yVelocity = rb.velocity.y;
        //if the player's y velocity is less than the speed boost ceiling, boost the player up
        if (yVelocity < speedBoostCeiling)
        {
            //set the player's y velocity to the speed boost ceiling
            rb.velocity = new Vector3(rb.velocity.x, speedBoostCeiling, rb.velocity.z);
        }
    }
    //function that boosts the player forward (not based on controls)
    void BoostForward()
    {
        //get the player's velocity z direction
        float zVelocity = rb.velocity.z;
        //if the player's z velocity is less than the speed boost ceiling, boost the player forward
        if (zVelocity < speedBoostCeiling)
        {
            //set the player's z velocity to the speed boost ceiling
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speedBoostCeiling);
        }
    }
    //function that boosts the player backward (not based on controls)
    void BoostBack()
    {
        //get the player's velocity in the x and z directions
        float zVelocity = rb.velocity.z;
        //if the player's x velocity is less than the speed boost ceiling, boost the player backward
        if (-zVelocity < speedBoostCeiling)
        {
            //set the player's z velocity to the negative speed boost ceiling
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -speedBoostCeiling);
        }
    }
    //function that boosts the player left (not based on controls)
    void BoostLeft()
    {
        //get the player's velocity in the x direction
        float xVelocity = rb.velocity.x;
        //if the player's x velocity is less than the speed boost ceiling, boost the player left
        if (-xVelocity > speedBoostCeiling)
        {
            //set the player's x velocity to the negative speed boost ceiling
            rb.velocity = new Vector3(-speedBoostCeiling, rb.velocity.y, rb.velocity.z);
        }
    }
    //function that boosts the player right (not based on controls)
    void BoostRight()
    {
        //get the player's velocity in the x direction
        float xVelocity = rb.velocity.x;
        //if the player's x velocity is less than the speed boost ceiling, boost the player right
        if (xVelocity < speedBoostCeiling)
        {
            //set the player's x velocity to the speed boost ceiling
            rb.velocity = new Vector3(speedBoostCeiling, rb.velocity.y, rb.velocity.z);
        }
    }
}
