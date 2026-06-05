using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance;
    private Image brightnessOverlay;
    private float maxDarkness = 0.6f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("GameSettingsManager");
        go.AddComponent<GameSettingsManager>();
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateBrightnessOverlay();
        ApplyAllSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void CreateBrightnessOverlay()
    {
        GameObject canvasGO = new GameObject("BrightnessCanvas");
        DontDestroyOnLoad(canvasGO);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        canvasGO.AddComponent<CanvasScaler>();

        GameObject imgGO = new GameObject("BrightnessOverlay");
        imgGO.transform.SetParent(canvasGO.transform, false);

        brightnessOverlay = imgGO.AddComponent<Image>();
        brightnessOverlay.color = new Color(0, 0, 0, 0);
        brightnessOverlay.raycastTarget = false;

        RectTransform rect = imgGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyBrightness();
    }

    public void ApplyAllSettings()
    {
        ApplyBrightness();
        ApplyVolume();
        ApplyFPS();
        ApplyFullscreen();
    }

    public void ApplyBrightness()
    {
        if (brightnessOverlay == null) return;
        float value = PlayerPrefs.GetFloat("Brightness", 1f);
        float alpha = Mathf.Lerp(maxDarkness, 0f, value);
        Color c = brightnessOverlay.color;
        c.a = alpha;
        brightnessOverlay.color = c;
    }

    public void ApplyVolume()
    {
        float value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = Mathf.Max(0.0001f, value);
    }

    public void ApplyFPS()
    {
        int index = Mathf.Clamp(PlayerPrefs.GetInt("FPS", 1), 0, 3);
        QualitySettings.vSyncCount = 0;
        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = -1; break;
        }
    }

    public void ApplyFullscreen()
    {
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = fullscreen;
    }
}