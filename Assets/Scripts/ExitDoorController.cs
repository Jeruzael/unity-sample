using UnityEngine;

public class ExitDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openHeight = 4f;
    public float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool shouldOpen = false;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    private void Update()
    {
        if (shouldOpen)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                openPosition,
                openSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        TryOpenDoor();
    }

    public void TryOpenDoor()
    {
        if (SimpleGameManager.Instance.hasKey)
        {
            shouldOpen = true;
            Debug.Log("Exit door opened!");
        }
        else
        {
            Debug.Log("The exit door is locked. Find the key first.");
        }
    }
}