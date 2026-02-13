using UnityEngine;

public class PlayerCam1 : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("References")]
    public Transform orientation;   // Empty object for movement forward direction
    public Transform playerBody;    // The root transform of your player (with Rigidbody)
    public Transform headBone;      // Mixamorig:Head from your FBX

    [Header("Rotation Limits")]
    public float maxVerticalAngle = 90f;
    public float minVerticalAngle = -90f;

    private float xRotation = 0f;   // Vertical rotation (pitch)
    private float yRotation = 0f;   // Horizontal rotation (yaw)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseInput();
        RotateCamera();
        RotatePlayerBody();
        RotateHead();
        UpdateOrientation();
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
    }

    private void RotateCamera()
    {
        // Camera follows both pitch and yaw
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void RotatePlayerBody()
    {
        // Rotate the entire player body to face the camera's horizontal direction
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    private void RotateHead()
    {
        // Head looks up/down based on camera pitch, with body's yaw
        if (headBone != null)
        {
            // Apply vertical rotation to head (limited range for realistic neck movement)
            float clampedHeadPitch = Mathf.Clamp(xRotation, -60f, 60f);
            headBone.rotation = Quaternion.Euler(clampedHeadPitch, yRotation, 0);
        }
    }

    private void UpdateOrientation()
    {
        // Orientation object determines movement direction (should match body rotation)
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
