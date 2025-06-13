using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Audio")]
    
    public AudioSource sfxSource;       // Reference to the sound effects AudioSource
    public AudioClip gameOverSound; 
    public GameObject GameOverMenu;
         
    
    public GameObject player;

    void Start()
    {
        
        if (GameOverMenu)
            GameOverMenu.SetActive(false);

    }

    public void TriggerGameOver()
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
        GameOverMenu.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*public void RestartFromCheckpoint()
    {
        Time.timeScale = 1f;
        if (CheckpointManager.HasCheckpoint())
        {
            // Move the player back to the checkpoint
            player.transform.position = CheckpointManager.GetCheckpointPosition();
            HealthPlayer hp = player.GetComponent<HealthPlayer>();
            if (hp != null)
            {
                hp.currentHealth = hp.maxHealth; // or partial restore
            }
            GameOverMenu.SetActive(false);
            Debug.Log("Respawned from checkpoint.");

            
        }
        else
        {
            // No checkpoint set, reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        Debug.Log("Restarted from last checkpoint (needs custom logic)");
    }*/

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
