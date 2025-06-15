using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes!

public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject victoryMenu; // Assign your VictoryMenu panel to this in the Inspector.

    // A method to show the victory menu
    public void ShowVictoryMenu()
    {
        if (victoryMenu != null)
        {
            victoryMenu.SetActive(true);
            Time.timeScale = 0f; // Optional: Pauses the game by stopping time.
        }
        else
        {
            Debug.LogError("Victory Menu is not assigned in the UIManager!");
        }
    }

    // This public method will be called by our button's OnClick event
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANT: Un-pause the game before leaving the scene.
        SceneManager.LoadScene("Menu"); // IMPORTANT: Replace "MainMenu" with the exact name of your main menu scene file.
    }
}