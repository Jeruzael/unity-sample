using UnityEngine;

public class ExitDoorTrigger : MonoBehaviour
{
    public ExitDoorController exitDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        exitDoor.TryOpenDoor();
    }
}