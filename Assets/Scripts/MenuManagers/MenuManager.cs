using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pausemenu;
    public GameObject optionsMenu;
    public SoundManager SoundManagerScript;
    public Animator sceneTransition;

    private PlayerMovement playerMovement;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void Sound()
    {
        SoundManagerScript.SelectAudio(0, 1);
    }

    public void StartTutorial()
    {
        Time.timeScale = 1f;
        StartCoroutine(StartTutorialCoroutine());
    }

    IEnumerator StartTutorialCoroutine()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
        sceneTransition.SetTrigger("Start");
    }

    public void StartNivel1()
    {
        StartCoroutine(StartNivel1Coroutine());
        Time.timeScale = 1f;
    }

    IEnumerator StartNivel1Coroutine()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
        sceneTransition.SetTrigger("Start");
    }

    public void StartNivel2()
    {
        StartCoroutine(StartNivel2Coroutine());
        Time.timeScale = 1f;
    }

    IEnumerator StartNivel2Coroutine()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(3);
        sceneTransition.SetTrigger("Start");
    }

    public void ExitGame()
    {
        StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator ExitGameCoroutine()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        Application.Quit();
        print("Cerrar juego");
    }

    public void ExitLevel()
    {
        Time.timeScale = 1f;
        StartCoroutine(ExitMainMenuCoroutine());
    }

    IEnumerator ExitMainMenuCoroutine()
    {
        sceneTransition.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
        sceneTransition.SetTrigger("Start");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenu.activeSelf)
            {
                CloseOptions();
            }
            else if (pausemenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        SoundManagerScript.SelectAudio(0, 1);
        pausemenu.SetActive(true);
        if (playerMovement != null)
            playerMovement.SetMovementLocked(true);
    }

    public void ResumeGame()
    {
        SoundManagerScript.SelectAudio(0, 1);
        pausemenu.SetActive(false);
        if (playerMovement != null)
            playerMovement.SetMovementLocked(false);
    }

    public void CloseOptions()
    {
        SoundManagerScript.SelectAudio(0, 1);
        optionsMenu.SetActive(false);
        pausemenu.SetActive(true);
    }
}