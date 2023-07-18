using UnityEngine;

public class SkateboardMovement : MonoBehaviour
{
    public float maxSpeed = 10.0f;              // The maximum speed of the skateboard
    public float acceleration = 2.0f;           // The rate at which the skateboard accelerates
    public float deceleration = 1.0f;           // The rate at which the skateboard decelerates
    public float maxTurnAngle = 30.0f;          // The maximum angle the skateboard can turn
    public float turnAcceleration = 100.0f;     // The rate at which the skateboard turns
    public float turnDeceleration = 50.0f;      // The rate at which the skateboard stops turning
    public float grindSpeedMultiplier = 0.5f;   // The multiplier for the skateboard's speed while grinding

    private Rigidbody rb;                       // The rigidbody component of the skateboard
    private float forwardInput;                 // The forward input axis from the controller
    private float turnInput;                    // The turn input axis from the controller
    private bool grinding;                      // Whether the skateboard is currently grinding

    // Start is called before the first frame update
    void Start()
    {
        // Get the rigidbody component of the skateboard
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the forward and turn input axes from the controller
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Check if the skateboard is grinding
        grinding = false;
        // ... (code to check for grinding)
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    void FixedUpdate()
    {
        // Apply a force to the skateboard based on the forward input axis
        float speedMultiplier = grinding ? grindSpeedMultiplier : 1.0f;
        float targetSpeed = forwardInput * maxSpeed * speedMultiplier;
        float currentSpeed = Vector3.Dot(rb.velocity, transform.forward);
        float accelerationRate = forwardInput > 0 ? acceleration : deceleration;
        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * Time.fixedDeltaTime);
        rb.AddForce(transform.forward * (newSpeed - currentSpeed), ForceMode.VelocityChange);

        // Apply a torque to the skateboard based on the turn input axis
        float targetTurnAngle = turnInput * maxTurnAngle;
        float currentTurnAngle = Vector3.Angle(transform.forward, rb.velocity) * Mathf.Sign(turnInput);
        float turnAccelerationRate = turnInput != 0 ? turnAcceleration : turnDeceleration;
        float newTurnAngle = Mathf.MoveTowards(currentTurnAngle, targetTurnAngle, turnAccelerationRate * Time.fixedDeltaTime);
        Quaternion newRotation = Quaternion.AngleAxis(newTurnAngle, Vector3.up);
        rb.MoveRotation(transform.rotation * newRotation);
    }
}
