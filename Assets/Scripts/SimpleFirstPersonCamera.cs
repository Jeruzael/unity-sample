using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFirstPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;
    public Transform cameraPoint;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (playerBody == null || cameraPoint == null)
        {
            return;
        }

        // Keep the camera at the player's eye position
        transform.position = cameraPoint.position;

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // Rotate player left/right
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        transform.localRotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
    }
}