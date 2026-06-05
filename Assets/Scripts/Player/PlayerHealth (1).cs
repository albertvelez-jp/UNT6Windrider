using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("UI (Raw Images)")]
    public RawImage[] lifeIcons;

    [Header("Animation Settings")]
    public float lifeDisappearDuration = 0.3f;

    [Header("Invincibility Settings")]
    private bool invincible = false;
    public float invincibilityTime = 1f;

    [Header("Death Effects")]
    public ParticleSystem deathParticles;
    public float respawnDelay = 1f;

    [Header("Blink Settings")]
    public float blinkDuration = 1f;
    public float blinkInterval = 0.1f;

    private Vector3 currentCheckpoint;
    private Vector3 startPosition;
    private bool hasCheckpoint = false;
    public bool isDead = false;

    private PlayerMovement pm;
    private Rigidbody rb;
    private SpriteRenderer[] spriteRenderers;
    private Renderer[] playerRenderers;
    public SoundManager SoundManagerScript;
    public AudioSource audioSourceAmbient;

    void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        audioSourceAmbient = GameObject.FindGameObjectWithTag("Ambient").GetComponent<AudioSource>();
        currentLives = maxLives;
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        playerRenderers = GetComponentsInChildren<Renderer>();

        ResetLifeUI();

        if (spriteRenderers.Length == 0 && playerRenderers.Length == 0)
            Debug.LogWarning("PlayerHealth: No se encontro ningun Renderer.");
        else
            Debug.Log("PlayerHealth: Renderers encontrados: " + playerRenderers.Length);
    }

    public void ActivateCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        hasCheckpoint = true;
        Debug.Log("Checkpoint activado en: " + position);
    }

    public void TakeDamage()
    {
        if (invincible || isDead) return;

        SoundManagerScript.SelectAudio(5, 1);
        currentLives--;

        if (currentLives >= 0 && currentLives < lifeIcons.Length)
            StartCoroutine(AnimateLifeDisappear(lifeIcons[currentLives]));

        if (currentLives > 0)
        {
            StartCoroutine(DeathSequence(false));
        }
        else
        {
            Debug.Log("Jugador sin vidas. Reiniciando desde inicio...");
            StopAllCoroutines();
            currentLives = maxLives;
            ResetLifeUI();
            hasCheckpoint = false;
            ResetAllCheckpoints();
            StartCoroutine(DeathSequence(true));
        }
    }

    private void ResetInvincibility()
    {
        invincible = false;
    }

    private IEnumerator DeathSequence(bool fullReset)
    {
        isDead = true; // Bloquea la cámara y el movimiento
        invincible = true;
        audioSourceAmbient.mute = true;

        if (pm != null) pm.inAirBoost = false;

        Vector3 respawnPos = fullReset ? startPosition : (hasCheckpoint ? currentCheckpoint : startPosition);

        SpawnDeathParticles();
        SetPlayerVisible(false);

        // TELEPORTE INMEDIATO: Evita fallos en bajos FPS
        // El jugador ya está en el checkpoint, pero es invisible y la cámara no lo sabe
        TeleportTo(respawnPos);

        // Forzamos que se quede quieto en el checkpoint durante la espera
        if (rb != null) rb.isKinematic = true;

        // ESPERA DE 1 SEGUNDO (La cámara sigue en el sitio de la muerte)
        yield return new WaitForSecondsRealtime(respawnDelay);

        // Restauramos físicas y estado
        if (rb != null) rb.isKinematic = false;

        audioSourceAmbient.mute = false;
        isDead = false; // La cámara ahora detecta la nueva posición y viaja hacia ella
        SetPlayerVisible(true);

        yield return StartCoroutine(BlinkEffect());

        invincible = true;
        Invoke(nameof(ResetInvincibility), invincibilityTime);
    }

    private void TeleportTo(Vector3 position)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        transform.position = position;
    }

    private void SpawnDeathParticles()
    {
        if (deathParticles == null) return;
        ParticleSystem ps = Instantiate(deathParticles, transform.position, Quaternion.identity);
        ps.Play();
        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    private IEnumerator BlinkEffect()
    {
        float elapsed = 0f;
        while (elapsed < blinkDuration)
        {
            SetPlayerVisible(false);
            yield return new WaitForSecondsRealtime(blinkInterval);
            SetPlayerVisible(true);
            yield return new WaitForSecondsRealtime(blinkInterval);
            elapsed += blinkInterval * 2f;
        }
        SetPlayerVisible(true);
    }

    private void SetPlayerVisible(bool visible)
    {
        if (spriteRenderers != null && spriteRenderers.Length > 0)
        {
            foreach (var sr in spriteRenderers)
                if (sr != null) sr.enabled = visible;
        }
        else if (playerRenderers != null && playerRenderers.Length > 0)
        {
            foreach (var r in playerRenderers)
                if (r != null) r.enabled = visible;
        }
    }

    private IEnumerator AnimateLifeDisappear(RawImage lifeImage)
    {
        if (lifeImage == null) yield break;
        Vector3 startScale = lifeImage.rectTransform.localScale;
        Vector3 endScale = Vector3.zero;
        float timer = 0f;
        while (timer < lifeDisappearDuration)
        {
            timer += Time.deltaTime;
            lifeImage.rectTransform.localScale = Vector3.Lerp(startScale, endScale, timer / lifeDisappearDuration);
            yield return null;
        }
        lifeImage.rectTransform.localScale = endScale;
        lifeImage.enabled = false;
    }

    private void ResetLifeUI()
    {
        if (lifeIcons == null || lifeIcons.Length == 0) return;
        foreach (var life in lifeIcons)
        {
            if (life != null)
            {
                life.enabled = true;
                life.rectTransform.localScale = Vector3.one;
            }
        }
    }

    private void ResetAllCheckpoints()
    {
        foreach (Checkpoint cp in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
            cp.ForceReset();
    }

    public void Reaparecer()
    {
        StartCoroutine(DeathSequence(false));
    }
}