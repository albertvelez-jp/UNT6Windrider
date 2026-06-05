using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private bool used = false;

    private ParticleSystem particles;
    private Collider checkpointCollider;
    public SoundManager SoundManagerScript;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        checkpointCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        SoundManagerScript.SelectAudio(4, 3);
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ActivateCheckpoint(transform.position);
            StartCoroutine(TemporaryDisable());
        }
    }

    private IEnumerator TemporaryDisable()
    {
        used = true;

        if (checkpointCollider != null) checkpointCollider.enabled = false;

        if (particles != null)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Ya no reaparece por tiempo: solo cuando el jugador pierda todas las vidas
        yield break;
    }

    // Llamado por PlayerHealth cuando el jugador pierde todas las vidas
    public void ForceReset()
    {
        StopAllCoroutines();
        used = false;

        if (checkpointCollider != null) checkpointCollider.enabled = true;

        if (particles != null)
            particles.Play();
    }
}