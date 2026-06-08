using TMPro;
using UnityEngine;

public class SimpleUIManager : MonoBehaviour
{
    public static SimpleUIManager Instance;

    [Header("UI References")]
    public TMP_Text objectiveText;
    public GameObject endPanel;
    public TMP_Text endText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowObjective("Objective: Find the key.");
        HideEndPanel();
    }

    public void ShowObjective(string message)
    {
        if (objectiveText != null)
        {
            objectiveText.text = message;
        }
    }

    public void ShowWinScreen()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endText != null)
        {
            endText.text = "You Win!";
        }
    }

    public void ShowLoseScreen()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (endText != null)
        {
            endText.text = "Game Over";
        }
    }

    public void HideEndPanel()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
    }
}