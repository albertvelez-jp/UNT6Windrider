using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialManager tutorialManager; // Referencia al TutorialManager
    public int stepIndex;                   // Paso que se activa en esta zona

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.StartTutorialStep(stepIndex);
            gameObject.SetActive(false); // Desactivar el trigger para que no se repita
        }
    }
}