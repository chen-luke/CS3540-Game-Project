using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float spawnRadius = 10f;
    public float firstSpawnTime = 2f;
    public int maxEnemies = 5;
    public int activeEnemies = 0;
    public AudioClip spawnSFX;

    // Start is called before the first frame update
    void Start()
    {
        activeEnemies = 0;
        InvokeRepeating("SpawnEnemy", firstSpawnTime, spawnRate);
    }

    void SpawnEnemy()
    {
        if (activeEnemies < maxEnemies)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 spawnPosition = transform.position + randomOffset;
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0));
            enemy.GetComponent<EnemyBehavior>().spawner = this;
            enemy.transform.SetParent(GameObject.FindGameObjectWithTag("EnemyParent").transform);
            AudioSource.PlayClipAtPoint(spawnSFX, spawnPosition);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }
}
