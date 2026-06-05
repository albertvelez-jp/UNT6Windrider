using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float slowMultiplier = 0.4f;  // 40% de velocidad
    public float slowDuration = 2f;      // 2 segundos ralentizado

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();

        if (pm != null)
        {
            pm.ApplySlow(slowMultiplier, slowDuration);

            // opcional: destruir el rayo tras golpear
            Destroy(gameObject, 0.05f);
        }
    }
}
