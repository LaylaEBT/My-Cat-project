using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int collectibleCount = 0;
    // We make the reference 'private' to prevent other scripts from changing it accidentally.
    private TextMeshProUGUI collectibleText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This will now run safely after each new scene loads.
        FindCollectibleText();
    }

    public void AddCollectible()
    {
        collectibleCount++;
        UpdateCollectibleUI();
    }

    void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = "x " + collectibleCount.ToString();
        }
    }

    void FindCollectibleText()
    {
        GameObject textObject = GameObject.Find("CollectibleText");
        if (textObject != null)
        {
            // Try to get the component.
            collectibleText = textObject.GetComponent<TextMeshProUGUI>();

            // *** THE NEW, SAFER CHECK! ***
            // If we successfully got the component, update the UI.
            if (collectibleText != null)
            {
                UpdateCollectibleUI();
            }
            else
            {
                // This error is more specific and helpful!
                Debug.LogError("Object 'CollectibleText' was found, but it is missing the TextMeshProUGUI component!");
            }
        }
        else
        {
            Debug.LogWarning("Could not find 'CollectibleText' object in this scene. Make sure it exists and is named correctly.");
        }
    }
}