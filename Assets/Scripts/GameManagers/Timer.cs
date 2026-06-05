using UnityEngine;
using TMPro;

public class Timer_TMP : MonoBehaviour
{
    public float timeRemaining = 30f;
    public TMP_Text timeText;
    public bool timerRunning = true;
    public bool playerWon = false;

    void Awake()
    {
        // No se destruye si recargas escena
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (timerRunning && !playerWon)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timeText.text = timeRemaining.ToString("0.0");
            }
            else
            {
                timeRemaining = 0;
                timerRunning = false;
                timeText.text = "¡Tiempo agotado!";
                Debug.Log("Has perdido");
            }
        }
    }

    public void PlayerReachedGoal()
    {
        playerWon = true;
        timerRunning = false;
        
    }
}