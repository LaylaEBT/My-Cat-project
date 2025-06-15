using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsPanel;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!settingsPanel.activeSelf)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (!isPaused)
            settingsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false); // Hide pause
        settingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}