using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    public float expandDistance = 10f;    // distancia máxima antes de regresar
    public float rotationSpeed = 50f;     // velocidad de rotación en grados por segundo

    private Vector3 initialPosition;
    private Vector3 direction;
    private float speed;
    private bool returning = false;       // false = expandirse, true = regresar

    void Start()
    {
        // Guardamos la posición inicial
        initialPosition = transform.position;

        // Dirección aleatoria inicial
        direction = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        ).normalized;

        // Velocidad aleatoria
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        // Movimiento de expansión/contracción
        if (!returning)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, initialPosition) >= expandDistance)
            {
                returning = true;
            }
        }
        else
        {
            Vector3 newDir = (initialPosition - transform.position).normalized;
            transform.Translate(newDir * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, initialPosition) <= 0.1f)
            {
                returning = false;

                // Nueva dirección aleatoria para la próxima expansión
                direction = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    0f
                ).normalized;
            }
        }

        // Rotación normal
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
