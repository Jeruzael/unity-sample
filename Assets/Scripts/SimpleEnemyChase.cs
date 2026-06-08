using UnityEngine;

public class SimpleEnemyChase : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Enemy Settings")]
    public float detectionRange = 8f;
    public float moveSpeed = 2.5f;
    public float catchDistance = 1.2f;

    private void Update()
    {
        if (SimpleGameManager.Instance.gameOver || SimpleGameManager.Instance.gameWon)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            ChasePlayer();
        }

        if (distance <= catchDistance)
        {
            SimpleGameManager.Instance.GameOver();
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 0.1f)
        {
            return;
        }

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.forward = direction;
    }
}