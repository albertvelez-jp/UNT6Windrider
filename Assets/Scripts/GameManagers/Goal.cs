using UnityEngine;

public class Goal : MonoBehaviour
{
    public GameObject victoryMenu;
    private CameraManager cameraManager;
    public SoundManager SoundManagerScript;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SoundManagerScript.SelectAudio(6, 1);
        Time.timeScale = 0.1f;
        victoryMenu.SetActive(true);

        if (cameraManager != null)
            cameraManager.enabled = false;
    }
}