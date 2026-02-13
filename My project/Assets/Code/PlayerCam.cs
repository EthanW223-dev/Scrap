using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;

    public Transform orientation;   // Empty object for movement forward
    public Transform playerBody;    // Mesh root (PlayerOBJ)
    public Transform headBone;      // Mixamorig:Head

    private float xRotation = 0f;   // pitch
    private float yRotation = 0f;   // yaw

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Camera rotation (pitch + yaw)
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Body rotates only with yaw (left/right)
        if (playerBody != null)
            playerBody.rotation = Quaternion.Euler(0, yRotation, 0);

        // Head follows full camera rotation
        if (headBone != null)
            headBone.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Orientation for movement
        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
