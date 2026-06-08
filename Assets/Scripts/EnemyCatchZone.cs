using UnityEngine;

public class EnemyCatchZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleGameManager.Instance.GameOver();
        }
    }
}