using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes!

public class VictoryManager : MonoBehaviour
{
    [Header("Audio")]
    
    public AudioSource sfxSource;       // Reference to the sound effects AudioSource
    public AudioClip gameOverSound; 
    public GameObject VictoryMenu;
         
    
    public GameObject player;

    void Start()
    {
        
        if (VictoryMenu)
            VictoryMenu.SetActive(false);

    }

    public void TriggerVictoryMenu()
    {
        /*if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
        AudioManager.Instance.musicSource.Stop();*/
        Debug.Log("TriggerGameOver called");
    
        if (AudioManager.Instance == null)
        {
          Debug.LogError("AudioManager.Instance is NULL!");
        }
        else
        {
           Debug.Log("AudioManager.Instance found");

        if (AudioManager.Instance.musicSource == null)
        {
            Debug.LogError("musicSource is NULL inside AudioManager!");
        }
        else
        {
            Debug.Log("musicSource found, stopping music now...");
            AudioManager.Instance.musicSource.Stop();
        }
    }

    // Play Game Over Sound
        if (sfxSource != null && gameOverSound != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }


    
        Time.timeScale = 0f; // Pause the game
        VictoryMenu.SetActive(true);
    }


    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
