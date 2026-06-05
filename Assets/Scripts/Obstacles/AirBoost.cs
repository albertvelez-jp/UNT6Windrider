using UnityEngine;
using System.Collections;

public class AirBoost : MonoBehaviour
{
    [Header("Boost Settings")]
    public float forwardBoostDistance = 24f;
    public float forwardBoostDuration = 0.5f;
    public float speedBonus = 5f;
    public float speedBonusHoldTime = 0.5f;
    public float speedBonusDecayTime = 1.5f;

    [Header("Center Pull Settings")]
    public float centerPullDuration = 0.4f;
    public Vector2 centerOffset = Vector2.zero;

    [Header("Particles")]
    public float particlesDuration = 1f;

    private Coroutine pullCoroutine;
    private SoundManager SoundManagerScript;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SoundManagerScript.SelectAudio(2, 45);

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        pm.EnterAirBoost(particlesDuration);

        pm.TriggerExternalForwardDash(forwardBoostDistance, forwardBoostDuration);

        float targetSpeed = pm.baseForwardSpeed + speedBonus;
        pm.AddBoostSpeed(targetSpeed, speedBonusHoldTime, speedBonusDecayTime);

        if (pullCoroutine != null) StopCoroutine(pullCoroutine);
        pullCoroutine = StartCoroutine(PullToCenter(pm));

        CameraManager camManager = Camera.main != null
            ? Camera.main.GetComponent<CameraManager>()
            : null;
        if (camManager != null)
            camManager.TriggerBoostEffect();
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        pm.inAirBoost = false;

        if (pullCoroutine != null)
        {
            StopCoroutine(pullCoroutine);
            pullCoroutine = null;
        }
    }

    private IEnumerator PullToCenter(PlayerMovement pm)
    {
        Rigidbody rb = pm.GetComponent<Rigidbody>();
        float elapsed = 0f;
        float startX = pm.transform.position.x;
        float startY = pm.transform.position.y;

        float targetX = transform.position.x + centerOffset.x;
        float targetY = transform.position.y + centerOffset.y;

        while (elapsed < centerPullDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / centerPullDuration);

            Vector3 currentPos = rb.position;
            currentPos.x = Mathf.Lerp(startX, targetX, t);
            currentPos.y = Mathf.Lerp(startY, targetY, t);
            rb.MovePosition(currentPos);

            yield return new WaitForFixedUpdate();
        }

        pullCoroutine = null;
    }
}