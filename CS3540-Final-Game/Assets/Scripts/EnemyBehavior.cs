using System;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public EnemySpawner spawner;
    void Start()
    {
        if (spawner)
        {
            spawner.activeEnemies++;
        }

    }
    private void OnDestroy()
    {
        if (spawner)
        {
            spawner.activeEnemies--;
        }
    }
}
