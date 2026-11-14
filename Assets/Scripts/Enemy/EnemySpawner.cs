using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawner")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxEnemiesInGame = 2;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnRadius = 3f;

    private float nextSpawnTime = 0f;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (enemyPrefab == null)
        {
            Debug.LogWarning("[Spawner] Falta asignar Enemy Prefab en el Inspector.");
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + spawnInterval;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        int currentEnemies = FindObjectsByType<EnemySentinel>(FindObjectsSortMode.None).Length;
        if (currentEnemies >= maxEnemiesInGame)
        {
            Debug.Log($"[Spawner] Límite alcanzado ({currentEnemies}/{maxEnemiesInGame}). Esperando que desaparezca uno.");
            return;
        }

        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0f;
        Vector3 candidatePos = transform.position + randomOffset;

        if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 25f, NavMesh.AllAreas))
        {
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            Debug.Log($"[Spawner] Enemigo generado. Activos ahora: {currentEnemies + 1}/{maxEnemiesInGame}");
        }
        else
        {
            Debug.LogWarning("[Spawner] No se encontró NavMesh cerca de la posición del Spawner. Asegurate de ubicarlo sobre la zona azul horneada.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}