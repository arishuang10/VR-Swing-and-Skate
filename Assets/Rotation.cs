using UnityEngine;
using Oculus;
using System.Security.Cryptography;

public class Rotation : MonoBehaviour
{
   
    public OVRCameraRig ovrCameraRig;
    /*
    void FixedUpdate()
    {
        // Get the OVRCameraRig's local rotation
        Quaternion cameraLocalRotation = ovrCameraRig.transform.localRotation;

        // Calculate the y-axis rotation of the OVRCameraRig relative to the player
        float cameraYRotation = cameraLocalRotation.eulerAngles.y;

        // Rotate the player around the y-axis
        transform.rotation = Quaternion.Euler(0f, cameraYRotation, 0f);
    }
    
    void FixedUpdate()
    {
        // Get the OVRCameraRig's center eye anchor
        Transform centerEyeAnchor = ovrCameraRig.centerEyeAnchor;

        // Get the y-axis rotation of the center eye anchor
        float cameraYRotation = centerEyeAnchor.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, cameraYRotation, 0f);

    }
    */

    void FixedUpdate()
    {
        // Get the OVRCameraRig's center eye anchor
        Transform centerEyeAnchor = ovrCameraRig.centerEyeAnchor;

        transform.LookAt(centerEyeAnchor.position);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }



}
