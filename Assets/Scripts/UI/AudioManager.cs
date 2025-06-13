using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip victoryMusic;

    [Header("Game Over Settings")]
    public GameObject gameOverMenu;
    public AudioClip gameOverSound; 
    private bool gameOverHandled = false;
    private bool isInGameplayScene = false;


    private float volume = 1f;
    private bool isMuted = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource.loop = true;

            LoadSettings();
            PlayMusicForScene(SceneManager.GetActiveScene().name);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);

        if (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Scene de Thibo")
        {
        isInGameplayScene = true;

        
        if (gameOverMenu == null)
        {
            gameOverMenu = GameObject.Find("GameOverMenu");
        }

        gameOverHandled = false; // Reset for new level
    }
    else
    {
        isInGameplayScene = false;
        gameOverMenu = null; // No GameOverMenu in non-gameplay scenes
    }
    }
    void Update()
    { 
        if (!isInGameplayScene)
           return; 

        if (gameOverMenu != null && gameOverMenu.activeSelf && !gameOverHandled)
        {
           HandleGameOverMusic();
        }
    }

    void PlayMusicForScene(string sceneName)
    {
    switch (sceneName)
    {
        case "Menu":
            musicSource.clip = mainMenuMusic;
            break;
        case "Level1":
            musicSource.clip = level1Music;
            break;
        case "Level2":
            musicSource.clip = level2Music;
            break;
        case "Scene de Thibo":
            musicSource.clip = level3Music;
            break;
        case "Victory":
            musicSource.clip = victoryMusic;
            break;
        default:
            musicSource.clip = null;
            break;
    }

    if (musicSource.clip != null)
    {
        musicSource.Play();
        ApplyVolume();
    }
    }

    private void HandleGameOverMusic()
    {
        if (musicSource != null)
    {
        musicSource.Stop(); // Stop the background music
    }

    if (gameOverSound != null)
    {
        musicSource.PlayOneShot(gameOverSound); // Play Game Over sound
    }

    gameOverHandled = true; // Prevent repeating this logic
    }


    void LoadSettings()
    {
        volume = PlayerPrefs.GetFloat("volume", 1f);
        isMuted = PlayerPrefs.GetInt("muted", 0) == 1;
    }

    void ApplyVolume()
    {
        musicSource.volume = isMuted ? 0f : volume;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        PlayerPrefs.SetFloat("volume", volume);
        ApplyVolume();
    }

    public void ToggleMute(bool mute)
    {
        isMuted = mute;
        PlayerPrefs.SetInt("muted", mute ? 1 : 0);
        ApplyVolume();
    }

    public float GetVolume() => volume;
    public bool IsMuted() => isMuted;
}