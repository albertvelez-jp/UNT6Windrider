using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgressBar : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider progressSlider;
    public TextMeshProUGUI percentText;

    [Header("Configuración del Nivel")]
    public Transform player;
    public float startZ; // Coordenada Z donde empieza el nivel
    public float endZ;   // Coordenada Z donde está la meta

    void Update()
    {
        if (player == null) return;

        // 1. Calculamos la distancia total y cuánto ha recorrido el jugador
        float totalDistance = endZ - startZ;
        float currentDistance = player.position.z - startZ;

        // 2. Calculamos el porcentaje (Clamp01 evita que baje de 0 o suba de 1)
        float progress = Mathf.Clamp01(currentDistance / totalDistance);
        float percentage = progress * 100f;

        // 3. Actualizamos el Slider (0 a 100)
        if (progressSlider != null)
        {
            progressSlider.value = percentage;
        }

        // 4. Actualizamos el Texto (con 0 decimales)
        if (percentText != null)
        {
            percentText.text = Mathf.FloorToInt(percentage) + "%";
        }
    }
}