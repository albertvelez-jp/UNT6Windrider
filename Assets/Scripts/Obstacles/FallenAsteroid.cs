using UnityEngine;

public class FallenAsteroid : MonoBehaviour
{
    [Header("Asteroid")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float asteroidLifetime = 15f;

    [Header("Spawn Time (Random)")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [Header("Movement")]
    [SerializeField] private Vector3 fallDirection = new Vector3(-1f, -1f, 0f);
    [SerializeField] private float asteroidSpeed = 10f;

    private float spawnTimer;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnAsteroid();
            ResetTimer();
        }
    }

    private void SpawnAsteroid()
    {
        if (asteroidPrefab == null)
            return;

        GameObject asteroid = Instantiate(
            asteroidPrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = fallDirection.normalized * asteroidSpeed;
        }

        Destroy(asteroid, asteroidLifetime);
    }

    private void ResetTimer()
    {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }
}
