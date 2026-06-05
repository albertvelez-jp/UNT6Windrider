using UnityEngine;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float spawnRate = 1f;

    public float spawnRadius = 5f;   // zona alrededor del spawner
    public float minDistance = 2f;   // separación mínima entre asteroides
    public int maxAsteroids = 15;    // número máximo permitido

    public int maxAttempts = 10;
    public Vector3 direction;
    public float speed;

    private List<Transform> spawnedAsteroids = new List<Transform>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroid), 1f, spawnRate);
    }
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

    }

    void SpawnAsteroid()
    {
        // ?? Si ya hay el máximo número de asteroides, no spawneamos más
        if (spawnedAsteroids.Count >= maxAsteroids)
            return;

        Vector3 spawnPos = Vector3.zero;
        bool foundSpot = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                Random.Range(-spawnRadius, spawnRadius),
                0f
            );

            bool tooClose = false;

            foreach (Transform asteroid in spawnedAsteroids)
            {
                if (Vector3.Distance(spawnPos, asteroid.position) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                foundSpot = true;
                break;
            }
        }

        GameObject newAsteroid = Instantiate(asteroidPrefab, foundSpot ? spawnPos : transform.position, Quaternion.identity);

        spawnedAsteroids.Add(newAsteroid.transform);
    }
}
