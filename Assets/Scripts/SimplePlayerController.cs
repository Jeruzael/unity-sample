using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Check if player is touching the ground
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // 2. Keep player grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 3. Read keyboard input using the NEW Input System
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            input.y += 1;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            input.y -= 1;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            input.x += 1;
        }

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            input.x -= 1;
        }

        // 4. Convert input into 3D movement
        //Vector3 move = new Vector3(input.x, 0f, input.y);
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        // Prevent faster diagonal movement
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // 5. Make player face movement direction
        /*if (move.magnitude >= 0.1f)
        {
            transform.forward = move;
        }*/

        controller.Move(move * moveSpeed * Time.deltaTime);

        // 6. Jump using the NEW Input System
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 7. Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            Debug.Log("Collected an object!");
            Destroy(other.gameObject);
        }
    }
}