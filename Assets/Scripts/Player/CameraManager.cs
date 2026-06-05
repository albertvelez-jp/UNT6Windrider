using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -6);
    public float followSpeed = 5f;

    [Header("Z Follow")]
    public float zFollowOffset = -6f;

    [Header("FOV Values")]
    public float normalFOV = 60f;
    public float boostFOV = 80f;

    [Header("FOV Speeds")]
    public float boostFOVSpeed = 10f;
    public float boostFOVRecoverySpeed = 2f;

    [Header("Shake Settings")]
    public float shakeAmount = 0.05f;
    public float shakeDuration = 0.15f;

    private float fixedY;
    private Camera cam;
    private float shakeTimer = 0f;
    private float targetFOV;
    private float fovSpeed;

    // Referencia al script de salud
    private PlayerHealth playerHealth;

    void Start()
    {
        if (target != null)
        {
            fixedY = transform.position.y;
            playerHealth = target.GetComponent<PlayerHealth>();
        }

        cam = GetComponent<Camera>();
        cam.fieldOfView = normalFOV;
        targetFOV = normalFOV;
        fovSpeed = boostFOVRecoverySpeed;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- CAMBIO CLAVE ---
        // Si el jugador está en su secuencia de muerte, no seguimos su posición
        if (playerHealth != null && playerHealth.isDead) return;

        // Rotación
        transform.rotation = Quaternion.LookRotation(target.forward, Vector3.up);

        // Seguimiento
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        desiredPosition.y = fixedY;
        desiredPosition.z = target.position.z + zFollowOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSpeed * Time.deltaTime);

        // Shake
        if (shakeTimer > 0f)
        {
            transform.position += Random.insideUnitSphere * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }

        // Volver a FOV normal tras boost
        if (Mathf.Abs(cam.fieldOfView - targetFOV) < 0.1f && targetFOV != normalFOV)
        {
            targetFOV = normalFOV;
            fovSpeed = boostFOVRecoverySpeed;
        }
    }

    public void TriggerBoostEffect()
    {
        shakeTimer = shakeDuration;
        targetFOV = boostFOV;
        fovSpeed = boostFOVSpeed;
    }

    public void TriggerShakeOnly()
    {
        shakeTimer = shakeDuration;
    }
}