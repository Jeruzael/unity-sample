using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleFlashlight : MonoBehaviour
{
    [Header("Flashlight Reference")]
    public Light flashlightLight;

    [Header("Settings")]
    public bool startOn = true;

    private bool isOn;

    private void Start()
    {
        // If no Light is assigned manually, try to get one from this object
        if (flashlightLight == null)
        {
            flashlightLight = GetComponent<Light>();
        }

        if (flashlightLight == null)
        {
            Debug.LogWarning("No Light component assigned to SimpleFlashlight.");
            return;
        }

        isOn = startOn;
        flashlightLight.enabled = isOn;
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        if (flashlightLight == null)
        {
            Debug.LogWarning("Flashlight Light is missing.");
            return;
        }

        isOn = !isOn;
        flashlightLight.enabled = isOn;

        Debug.Log("Flashlight is now: " + (isOn ? "ON" : "OFF"));
    }
}