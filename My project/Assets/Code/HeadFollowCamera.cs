using UnityEngine;

public class HeadFollowCamera : MonoBehaviour
{
    public Transform headBone;       // Assign Mixamorig:Head
    public Transform cameraTransform; // Assign the camera with PlayerCam

    void LateUpdate()
    {
        if (headBone != null && cameraTransform != null)
        {
            headBone.rotation = cameraTransform.rotation;
        }
    }
}
