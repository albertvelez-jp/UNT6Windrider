using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public Slider brightnessSlider;
    public Slider volumeSlider;
    public TMP_Dropdown fpsDropdown;
    public Toggle fullscreenToggle;

    private void Start()
    {
        LoadSettings();

        brightnessSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
        fpsDropdown.onValueChanged.RemoveAllListeners();
        fullscreenToggle.onValueChanged.RemoveAllListeners();

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fpsDropdown.onValueChanged.AddListener(SetFPS);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    #region Brightness
    void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
        GameSettingsManager.Instance.ApplyBrightness();
    }
    #endregion

    #region Volume
    void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        GameSettingsManager.Instance.ApplyVolume();
    }
    #endregion

    #region FPS
    void SetFPS(int index)
    {
        PlayerPrefs.SetInt("FPS", index);
        GameSettingsManager.Instance.ApplyFPS();
    }
    #endregion

    #region Fullscreen
    void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        GameSettingsManager.Instance.ApplyFullscreen();
    }
    #endregion

    void LoadSettings()
    {
        float savedBrightness = Mathf.Clamp(PlayerPrefs.GetFloat("Brightness", 1f), 0f, 1f);
        float savedVolume = Mathf.Clamp(PlayerPrefs.GetFloat("Volume", 1f), 0f, 1f);
        int savedFPS = PlayerPrefs.GetInt("FPS", 1);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        brightnessSlider.value = savedBrightness;
        volumeSlider.value = savedVolume;
        fpsDropdown.value = savedFPS;
        fullscreenToggle.isOn = savedFullscreen;
    }
}