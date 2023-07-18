using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //reference to the player physics gameobject
    public GameObject physicalPlayer;
    public float yOffset = 1.0f; // The displacement on the y-axis from the cylinder's position
    //the transform component of the cylinder
    private Transform cylinderTransform;
    //function that runs at the start of the game
    void Start()
    {
        // Get a reference to the transform component of the cylinder
        cylinderTransform = physicalPlayer.GetComponent<Transform>();
    }
    void LateUpdate()
    {
        // Set the camera's position to the cylinder's position with the specified y-offset
        transform.position = cylinderTransform.position + new Vector3(0, yOffset, 0);
    }
}
