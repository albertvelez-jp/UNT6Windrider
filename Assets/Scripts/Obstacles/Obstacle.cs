using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Penalties")]
    public float slowTime = 0.75f;      // Tiempo en bajar a 0
    public float recoverTime = 1.2f;      // Tiempo en volver a la velocidad normal

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (pm != null)
            pm.AddTemporaryForwardSpeed(0f, slowTime, recoverTime);

        if (ph != null)
            ph.TakeDamage();
    }
}