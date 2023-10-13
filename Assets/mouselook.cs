using UnityEngine;

public class mouselook : MonoBehaviour
{
    public float sensitivity = 2.0f;      // Mouse sensitivity
    public float maxYAngle = 80.0f;       // Maximum vertical rotation angle

    private float rotationX = 0.0f;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
       // Cursor.visible = false;
    }
    private void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate rotation amount based on mouse input
        rotationX -= mouseY * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -maxYAngle, maxYAngle);  // Limit vertical rotation

        // Rotate the camera both horizontally and vertically
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.parent.rotation *= Quaternion.Euler(0, mouseX * sensitivity, 0);
    }
}
