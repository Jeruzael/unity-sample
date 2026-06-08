using UnityEngine;

public class SimpleGameManager : MonoBehaviour
{
    public static SimpleGameManager Instance;

    [Header("Game State")]
    public bool hasKey = false;
    public bool gameOver = false;
    public bool gameWon = false;

    private void Awake()
    {
        Instance = this;
    }

    public void CollectKey()
    {
        if (gameOver || gameWon)
        {
            return;
        }

        hasKey = true;
        Debug.Log("Key collected! Find the exit door.");
    }

    public void WinGame()
    {
        if (gameOver)
        {
            return;
        }

        gameWon = true;
        Debug.Log("You reached the safe room. You win!");
    }

    public void GameOver()
    {
        if (gameWon)
        {
            return;
        }

        gameOver = true;
        Debug.Log("Enemy caught you. Game over!");
    }
}