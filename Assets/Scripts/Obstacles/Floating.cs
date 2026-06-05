using UnityEngine;

public class Floating : MonoBehaviour
{
    public float amplitude = 0.2f; // Qué tanto se mueve (pequeño)
    public float speed = 1f;       // Qué tan rápido

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPos + new Vector3(0f, yOffset, 0f);
    }
}
