using UnityEngine;

public class WhirlwindController : MonoBehaviour
{
    [Header("Órbita Aleatoria")]
    public Transform anchorStone;
    public float orbitSpeed;
    public float radius = 10f;
    private float angle = 0f;

    [Header("Succión Lateral (Solo X)")]
    public float attractionForce = 300f; // Fuerza bruta lateral
    public float suctionPower = 10f;     // Velocidad de arrastre directo
    public float stickiness = 10f;       // Freno (Drag)
    public float breakTime = 0.8f;
    public string playerTag = "Player";

    private bool absorptionBlocked = false;
    private float unblockTimer = 0f;
    private Rigidbody playerRb;
    private PlayerMovement playerScript;
    private float originalDrag;

    void Start()
    {
        orbitSpeed = Random.Range(80f, 120f);
        angle = Random.Range(0f, 360f);
    }

    void Update()
    {
        if (anchorStone != null)
        {
            angle += orbitSpeed * Time.deltaTime;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            transform.position = anchorStone.position + new Vector3(x, 0, z);
        }

        if (absorptionBlocked)
        {
            unblockTimer -= Time.deltaTime;
            if (unblockTimer <= 0f) absorptionBlocked = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerScript = other.GetComponentInParent<PlayerMovement>();
        playerRb = other.GetComponentInParent<Rigidbody>();

        if (playerRb != null)
        {
            originalDrag = playerRb.linearDamping;
            playerRb.linearDamping = stickiness;
        }

        if (playerScript != null) playerScript.currentWhirlwind = this;
    }

    private void OnTriggerStay(Collider other)
    {
        if (absorptionBlocked || playerRb == null) return;

        // 1. Calculamos la dirección lateral (Solo X)
        // Restamos las posiciones pero ignoramos Y y Z para la fuerza
        float diffX = transform.position.x - playerRb.position.x;
        Vector3 forceDirection = new Vector3(diffX, 0, 0).normalized;

        // Aplicamos fuerza BRUTA solo en el eje X
        playerRb.AddForce(forceDirection * attractionForce, ForceMode.Acceleration);

        // 2. ARRASTRE DIRECTO (Solo X)
        // Calculamos la nueva posición X deseada (el centro del tornado)
        float targetX = Mathf.Lerp(playerRb.position.x, transform.position.x, Time.deltaTime * suctionPower);

        // Mantenemos la Y y la Z actuales del jugador intactas
        playerRb.MovePosition(new Vector3(targetX, playerRb.position.y, playerRb.position.z));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        ReleasePlayer();
    }

    public void BreakAbsorption()
    {
        absorptionBlocked = true;
        unblockTimer = breakTime;
        ReleasePlayer();
    }

    private void ReleasePlayer()
    {
        if (playerRb != null) playerRb.linearDamping = originalDrag;
        if (playerScript != null && playerScript.currentWhirlwind == this)
            playerScript.currentWhirlwind = null;
    }
}