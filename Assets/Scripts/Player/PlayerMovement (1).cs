using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 25f;
    public float baseForwardSpeed;
    public float runtimeForwardSpeed = 0f;

    [Header("Vertical Movement (W/S)")]
    public float verticalSpeed = 12f;
    public float minY = -3f;
    public float maxY = 5f;

    [Header("Lateral Movement (A/D)")]
    public float lateralSpeed = 12f;
    public float minX = -20f;
    public float maxX = 20f;

    [Header("Movement Smoothing")]
    public float movementSmoothing = 15f;
    public float inputSmoothing = 10f;
    public float borderBrakingDistance = 1.5f;

    [Header("Dash Settings (Mouse0)")]
    public float dashDistance = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 0f;
    private float dashTimer = 0f;
    private bool isDashing = false;
    private Vector3 dashVelocity;

    [Header("Visual Rotation Settings")]
    public float rollDuration = 0.5f;
    public AnimationCurve rollCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private Coroutine rollCoroutine;

    [Header("Forward Dash (Space)")]
    public float forwardDashDistance = 5f;
    public float forwardDashDuration = 0.1f;
    public float forwardDashCooldown = 1f;
    private float forwardDashTimer = 0f;
    public bool isForwardDashing = false;
    public float forwardDashSpeed = 0f;
    public bool inAirBoost = false;

    [Header("Air Boost Particles")]
    public ParticleSystem airBoostParticles;

    [Header("Sistema de ralentizacion")]
    private Coroutine slowCoroutine = null;
    private float originalForwardSpeed;
    private float originalLateralSpeed;
    private float originalVerticalSpeed;

    // Bloqueo de movimiento (pausa sin congelar el tiempo)
    private bool isMovementLocked = false;

    private float rawHorizontal;
    private float rawVertical;
    private float smoothedH;
    private float smoothedV;

    private Vector3 desiredMoveDirection;

    private Rigidbody rb;
    private PlayerHealth playerHealth;
    private SoundManager SoundManagerScript;

    private Coroutine tempSpeedCoroutine = null;

    [Header("Whirlwind Absorption")]
    public WhirlwindController currentWhirlwind;

    public float baseSpeed = 5f;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        baseForwardSpeed = forwardSpeed;
        runtimeForwardSpeed = baseForwardSpeed;

        currentSpeed = baseSpeed;
        playerHealth = GetComponent<PlayerHealth>();
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    /// <summary>
    /// Bloquea o desbloquea todo el movimiento del jugador sin alterar Time.timeScale.
    /// </summary>
    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;

        if (locked)
        {
            // Cancelar dashes activos inmediatamente
            isDashing = false;
            dashVelocity = Vector3.zero;
            isForwardDashing = false;
            forwardDashSpeed = 0f;
            inAirBoost = false;

            // Resetear inputs suavizados para que no haya inercia al reanudar
            smoothedH = 0f;
            smoothedV = 0f;
            rawHorizontal = 0f;
            rawVertical = 0f;
        }
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.isDead) return;

        // Si el movimiento esta bloqueado, no leemos inputs ni procesamos dashes
        if (isMovementLocked) return;

        if (inAirBoost)
        {
            rawHorizontal = 0f;
            rawVertical = 0f;
        }
        else
        {
            rawHorizontal = Input.GetAxisRaw("Horizontal");
        }

        rawVertical = Input.GetAxisRaw("Vertical");

        smoothedH = Mathf.Lerp(smoothedH, rawHorizontal, inputSmoothing * Time.deltaTime);
        smoothedV = Mathf.Lerp(smoothedV, rawVertical, inputSmoothing * Time.deltaTime);

        desiredMoveDirection = new Vector3(
            inAirBoost ? 0f : smoothedH * lateralSpeed,
            smoothedV * verticalSpeed,
            0f
        );

        if (!isDashing && dashTimer <= 0f && !inAirBoost)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 dashDir = Vector3.zero;
                bool hasLateralInput = false;
                float rollDirection = 0f;

                if (Input.GetKey(KeyCode.A)) { dashDir += Vector3.left; hasLateralInput = true; rollDirection = 360f; }
                if (Input.GetKey(KeyCode.D)) { dashDir += Vector3.right; hasLateralInput = true; rollDirection = -360f; }
                if (Input.GetKey(KeyCode.W)) dashDir += Vector3.up;
                if (Input.GetKey(KeyCode.S)) dashDir += Vector3.down;

                if (dashDir != Vector3.zero)
                {
                    SoundManagerScript.SelectAudio(1, 24);
                    StartDash(dashDir.normalized);

                    if (hasLateralInput)
                    {
                        if (rollCoroutine != null) StopCoroutine(rollCoroutine);
                        rollCoroutine = StartCoroutine(DoRoll(rollDirection));
                    }

                    if (currentWhirlwind != null)
                        currentWhirlwind.BreakAbsorption();
                }
            }
        }

        if (!isForwardDashing && forwardDashTimer <= 0f && !inAirBoost && Input.GetKeyDown(KeyCode.Space))
        {
            StartForwardDash();
            if (currentWhirlwind != null)
                currentWhirlwind.BreakAbsorption();
        }

        if (dashTimer > 0f) dashTimer -= Time.deltaTime;
        if (forwardDashTimer > 0f) forwardDashTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (playerHealth != null && playerHealth.isDead)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Si el movimiento esta bloqueado, detenemos al jugador completamente
        if (isMovementLocked)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        float currentForwardSpeed = isForwardDashing ? forwardDashSpeed : runtimeForwardSpeed;

        desiredMoveDirection = new Vector3(
            inAirBoost ? 0f : smoothedH * lateralSpeed,
            inAirBoost ? 0f : smoothedV * verticalSpeed, 0f);

        Vector3 targetVelocity = new Vector3(
            desiredMoveDirection.x,
            desiredMoveDirection.y,
            currentForwardSpeed
        );

        if (isDashing)
            targetVelocity.x = dashVelocity.x;

        Vector3 smoothVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, movementSmoothing * Time.fixedDeltaTime);

        float distToMinX = rb.position.x - minX;
        float distToMaxX = maxX - rb.position.x;
        float distToMinY = rb.position.y - minY;
        float distToMaxY = maxY - rb.position.y;

        if (distToMinX < borderBrakingDistance && smoothVelocity.x < 0f)
            smoothVelocity.x *= Mathf.Clamp01(distToMinX / borderBrakingDistance);
        if (distToMaxX < borderBrakingDistance && smoothVelocity.x > 0f)
            smoothVelocity.x *= Mathf.Clamp01(distToMaxX / borderBrakingDistance);
        if (distToMinY < borderBrakingDistance && smoothVelocity.y < 0f)
            smoothVelocity.y *= Mathf.Clamp01(distToMinY / borderBrakingDistance);
        if (distToMaxY < borderBrakingDistance && smoothVelocity.y > 0f)
            smoothVelocity.y *= Mathf.Clamp01(distToMaxY / borderBrakingDistance);

        rb.linearVelocity = smoothVelocity;
    }

    public void EnterAirBoost(float particlesDuration = 1f)
    {
        inAirBoost = true;
        if (airBoostParticles != null)
        {
            airBoostParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = airBoostParticles.main;
            main.duration = particlesDuration;
            airBoostParticles.Play();
        }
    }

    private IEnumerator DoRoll(float amount)
    {
        float elapsed = 0f;
        Vector3 currentEuler = transform.rotation.eulerAngles;
        float startZ = currentEuler.z;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rollDuration;
            float curveValue = rollCurve.Evaluate(t);
            float zRotation = Mathf.Lerp(0, amount, curveValue);
            transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, startZ + zRotation);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, startZ);
    }

    private void StartDash(Vector3 direction)
    {
        isDashing = true;
        dashTimer = dashCooldown;
        dashVelocity = direction * (dashDistance / dashDuration);

        CameraManager camManager = Camera.main != null ? Camera.main.GetComponent<CameraManager>() : null;
        if (camManager != null) camManager.TriggerShakeOnly();

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        dashVelocity = Vector3.zero;
    }

    private void StartForwardDash()
    {
        SoundManagerScript.SelectAudio(1, 24);
        isForwardDashing = true;
        forwardDashTimer = forwardDashCooldown;
        forwardDashSpeed = forwardDashDistance / forwardDashDuration;

        CameraManager camManager = Camera.main != null ? Camera.main.GetComponent<CameraManager>() : null;
        if (camManager != null) camManager.TriggerBoostEffect();

        StartCoroutine(ForwardDashRoutine(forwardDashDuration));
    }

    private IEnumerator ForwardDashRoutine(float customDuration)
    {
        yield return new WaitForSeconds(customDuration);
        isForwardDashing = false;
        forwardDashSpeed = 0f;
    }

    public void TriggerExternalForwardDash(float dashDistance, float dashDuration)
    {
        StopCoroutineIfExists(tempSpeedCoroutine);

        isForwardDashing = true;
        forwardDashTimer = dashDuration;
        forwardDashSpeed = dashDistance / Mathf.Max(0.0001f, dashDuration);

        CameraManager camManager = Camera.main != null ? Camera.main.GetComponent<CameraManager>() : null;
        if (camManager != null) camManager.TriggerBoostEffect();

        StartCoroutine(ForwardDashRoutine(dashDuration));
    }

    public void AddTemporaryForwardSpeed(float targetSpeed, float slowTime, float recoverTime)
    {
        if (tempSpeedCoroutine != null) StopCoroutine(tempSpeedCoroutine);
        tempSpeedCoroutine = StartCoroutine(TemporarySpeedRoutine(targetSpeed, slowTime, recoverTime));
    }

    private IEnumerator TemporarySpeedRoutine(float targetSpeed, float slowTime, float recoverTime)
    {
        float speedAtImpact = runtimeForwardSpeed;

        float timer = 0f;
        while (timer < slowTime)
        {
            timer += Time.deltaTime;
            runtimeForwardSpeed = Mathf.Lerp(speedAtImpact, targetSpeed, timer / slowTime);
            yield return null;
        }
        runtimeForwardSpeed = targetSpeed;

        timer = 0f;
        while (timer < recoverTime)
        {
            timer += Time.deltaTime;
            runtimeForwardSpeed = Mathf.Lerp(targetSpeed, baseForwardSpeed, timer / recoverTime);
            yield return null;
        }

        runtimeForwardSpeed = baseForwardSpeed;
        tempSpeedCoroutine = null;
    }

    private void StopCoroutineIfExists(Coroutine c)
    {
        if (c != null) StopCoroutine(c);
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(slowMultiplier, duration));
    }

    private IEnumerator SlowRoutine(float slowMultiplier, float duration)
    {
        originalForwardSpeed = runtimeForwardSpeed;
        originalLateralSpeed = lateralSpeed;
        originalVerticalSpeed = verticalSpeed;

        runtimeForwardSpeed *= slowMultiplier;
        lateralSpeed *= slowMultiplier;
        verticalSpeed *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        runtimeForwardSpeed = originalForwardSpeed;
        lateralSpeed = originalLateralSpeed;
        verticalSpeed = originalVerticalSpeed;

        slowCoroutine = null;
    }

    public void ModifySpeed(float factor) { currentSpeed = baseSpeed * factor; }
    public void ResetSpeed() { currentSpeed = baseSpeed; }

    public void AddBoostSpeed(float targetSpeed, float holdTime, float decayTime)
    {
        if (tempSpeedCoroutine != null) StopCoroutine(tempSpeedCoroutine);
        tempSpeedCoroutine = StartCoroutine(BoostSpeedRoutine(targetSpeed, holdTime, decayTime));
    }

    private IEnumerator BoostSpeedRoutine(float targetSpeed, float holdTime, float decayTime)
    {
        runtimeForwardSpeed = targetSpeed;

        yield return new WaitForSeconds(holdTime);

        float timer = 0f;
        while (timer < decayTime)
        {
            timer += Time.deltaTime;
            runtimeForwardSpeed = Mathf.Lerp(targetSpeed, baseForwardSpeed, timer / decayTime);
            yield return null;
        }

        runtimeForwardSpeed = baseForwardSpeed;
        tempSpeedCoroutine = null;
    }
}