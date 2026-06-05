using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1; // Cuánto vale esta moneda
    public GameObject collectEffect; // Opcional: Partículas al cogerla
    public SoundManager SoundManagerScript;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si es el jugador mediante el tag o el script de salud/movimiento
        if (other.CompareTag("Player"))
        {
            SoundManagerScript.SelectAudio(3, 8);
            // Sumar la moneda al GameManager
            GameManager.instance.AddCoins(coinValue);

            // Efecto visual opcional
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Destruir la moneda
            Destroy(gameObject);
        }
    }
}