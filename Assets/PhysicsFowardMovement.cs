using UnityEngine;

public class PhysicsForwardMovement : MonoBehaviour
{
    public float forwardSpeed = 1.0f;
    public float sidewaysSpeed = 0.5f;

    public Transform leftHand;
    public Transform rightHand;

    private Rigidbody playerRigidbody;
    private Vector3 previousLeftHandPosition;
    private Vector3 previousRightHandPosition;

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        previousLeftHandPosition = leftHand.position;
        previousRightHandPosition = rightHand.position;
    }

    private void Update()
    {
        Vector3 currentLeftHandPosition = leftHand.position;
        Vector3 currentRightHandPosition = rightHand.position;
        Vector3 leftHandVelocity = (currentLeftHandPosition - previousLeftHandPosition) / Time.deltaTime;
        Vector3 rightHandVelocity = (currentRightHandPosition - previousRightHandPosition) / Time.deltaTime;

        float leftRightMovement = (leftHandVelocity.x + rightHandVelocity.x) / 2;
        float forwardMovement = Mathf.Max(leftHandVelocity.z, rightHandVelocity.z);

        float speed = forwardMovement > 0 ? forwardSpeed : sidewaysSpeed;
        Vector3 movementVector = new Vector3(leftRightMovement * speed, 0, forwardMovement * speed);

        playerRigidbody.AddForce(movementVector, ForceMode.VelocityChange);

        previousLeftHandPosition = currentLeftHandPosition;
        previousRightHandPosition = currentRightHandPosition;
    }
}
