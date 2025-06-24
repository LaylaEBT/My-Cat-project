using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsPanel;

    public PlayerInput playerInput;
    public GameObject pauseFirstButton;

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

    //void OnEnable()
//{
    //EventSystem.current.SetSelectedGameObject(firstSelectedButton);
//}

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // PAUSE THE GAME
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            playerInput.SwitchCurrentActionMap("UI"); // Switch to UI controls
            
            // Clear any old selection and set the new one
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
        else
        {
            // RESUME THE GAME
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            settingsPanel.SetActive(false); // Also close settings on resume
            playerInput.SwitchCurrentActionMap("Player"); // Switch back to Gameplay controls
        }
    }
    

    public void ResumeGame()
   {
    /*isPaused = false;
    pauseMenu.SetActive(false);
    settingsPanel.SetActive(false);
    Time.timeScale = 1f;*/
    TogglePause();
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
    playerInput.SwitchCurrentActionMap("Player");
    SceneManager.LoadScene("Menu");
}
}
