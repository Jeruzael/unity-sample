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

    private void Start()
    {
        if (SimpleUIManager.Instance != null)
        {
            SimpleUIManager.Instance.ShowObjective("Objective: Find the key.");
        }
    }

    public void CollectKey()
    {
        if (gameOver || gameWon)
        {
            return;
        }

        hasKey = true;
        Debug.Log("Key collected! Find the exit door.");

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlaySound(SimpleAudioManager.Instance.keyPickupSound);
        }

        if (SimpleUIManager.Instance != null)
        {
            SimpleUIManager.Instance.ShowObjective("Key collected! Open the exit door.");
        }
    }

    public void DoorOpened()
    {
        if (gameOver || gameWon)
        {
            return;
        }

        if (SimpleUIManager.Instance != null)
        {
            SimpleUIManager.Instance.ShowObjective("Exit opened! Reach the safe room.");
        }

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlaySound(SimpleAudioManager.Instance.doorOpenSound);
        }
    }

    public void WinGame()
    {
        if (gameOver || gameWon)
        {
            return;
        }

        gameWon = true;
        Debug.Log("You reached the safe room. You win!");

        if (SimpleUIManager.Instance != null)
        {
            SimpleUIManager.Instance.ShowWinScreen();
        }

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlaySound(SimpleAudioManager.Instance.winSound);
        }
    }

    public void GameOver()
    {
        if (gameWon || gameOver)
        {
            return;
        }

        gameOver = true;
        Debug.Log("Enemy caught you. Game over!");

        if (SimpleUIManager.Instance != null)
        {
            SimpleUIManager.Instance.ShowLoseScreen();
        }

        if (SimpleAudioManager.Instance != null)
        {
            SimpleAudioManager.Instance.PlaySound(SimpleAudioManager.Instance.enemyCatchSound);
        }
    }
}