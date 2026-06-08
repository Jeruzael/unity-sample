using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "SampleScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}