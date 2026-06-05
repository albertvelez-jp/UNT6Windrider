using UnityEngine;

public class TutorialoFinal : MonoBehaviour
{
    public TutorialManager tutorialManager; // Referencia al TutorialManager

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialManager.CompleteStep();
            gameObject.SetActive(false); // Desactivar el trigger para que no se repita
        }
    }
}
