using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle soundToggle;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        soundToggle.onValueChanged.AddListener(ToggleSound);

        // Load saved settings if needed
        volumeSlider.value = AudioListener.volume;
        soundToggle.isOn = AudioListener.volume > 0;
    }

    void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    void ToggleSound(bool isOn)
    {
        AudioListener.volume = isOn ? volumeSlider.value : 0f;
    }
}