using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string instructionText;   // Texto que aparecerá
        public Sprite instructionImage;  // Imagen explicativa
        public KeyCode mainKey;          // Tecla principal (W, A, S, D, etc.)
        public KeyCode actionKey;        // Tecla de acción (F, Space, etc.), opcional
    }

    [Header("Referencias UI")]
    public TextMeshProUGUI displayText; // Texto TMP
    public GameObject panelFondo;       // Panel detrás del texto
    public Image displayImage;          // Imagen explicativa

    [Header("Pasos del Tutorial")]
    public TutorialStep[] steps;

    private int currentStep = -1;       // -1 = tutorial inactivo
    private bool stepActive = false;    // true = esperando acción del jugador

    void Update()
    {
        if (!stepActive || currentStep < 0) return;

        TutorialStep step = steps[currentStep];
        bool keyPressed = false;

        if (step.actionKey == KeyCode.None) // Solo tecla principal
        {
            keyPressed = Input.GetKeyDown(step.mainKey);
        }
        else // Combinación mainKey + actionKey
        {
            keyPressed = Input.GetKey(step.mainKey) && Input.GetKeyDown(step.actionKey);
        }

        if (keyPressed)
        {
            CompleteStep();
        }
    }

    /// <summary>
    /// Llama a este método desde un trigger para iniciar un paso de tutorial
    /// </summary>
    /// <param name="stepIndex">Índice del paso en el array steps</param>
    public void StartTutorialStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= steps.Length) return;

        currentStep = stepIndex;
        stepActive = true;

        // Pausar el juego
        Time.timeScale = 0.15f;

        // Mostrar UI
        ShowStep(currentStep);
    }

    private void ShowStep(int stepIndex)
    {
        TutorialStep step = steps[stepIndex];

        // Activar panel y texto
        panelFondo.SetActive(true);
        displayText.text = step.instructionText;

        // Activar imagen
        if (step.instructionImage != null)
        {
            displayImage.sprite = step.instructionImage;
            displayImage.gameObject.SetActive(true);
        }
        else
        {
            displayImage.gameObject.SetActive(false);
        }
    }

    public void CompleteStep()
    {
        stepActive = false;

        // Ocultar UI
        panelFondo.SetActive(false);
        displayText.text = "";
        displayImage.gameObject.SetActive(false);

        // Reanudar juego
        Time.timeScale = 1f;
    }
}
