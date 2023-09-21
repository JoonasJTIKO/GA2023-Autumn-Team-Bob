using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemySpawning : MonoBehaviour
    {
        [SerializeField]
        private EnemySpawnPoint[] spawnPoints;

        [SerializeField]
        private PlayerUnit player;

        [SerializeField]
        private Transform[] enemyPrefabs;

        private ComponentPool<Transform>[] enemyPools = new ComponentPool<Transform>[5];

        private void Awake()
        {
            int i = 0;
            foreach (Transform enemyPrefab in enemyPrefabs)
            {
                enemyPools[i] = new ComponentPool<Transform>(enemyPrefab.transform, 50);
                i++;
            }
        }

        public void SpawnEnemies(WaveData.WaveEnemy enemy, int amount)
        {
            if (amount / enemy.spawnGroupSize > spawnPoints.Length)
            {
                Debug.LogError("Can not spawn more enemy groups than there are spawn points!");
                return;
            }

            int unallocatedAmount = amount;

            List<EnemySpawnPoint> selectedPoints = new List<EnemySpawnPoint>();

            while (unallocatedAmount > 0)
            {
                EnemySpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                if (!selectedPoints.Contains(spawnPoint))
                {
                    selectedPoints.Add(spawnPoint);

                    int amountToSpawn = enemy.spawnGroupSize;
                    if (unallocatedAmount - enemy.spawnGroupSize < 0)
                    {
                        amountToSpawn = unallocatedAmount;
                    }

                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        spawnPoint.QueueEnemy(enemy);
                    }

                    unallocatedAmount -= amountToSpawn;
                }
            }
        }

        public void SpawnEnemy(WaveData.WaveEnemy enemy, Vector3 position)
        {
            int index = 0;
            switch (enemy.EnemyType)
            {
                case WaveData.EnemyType.MeleeStandard:
                    index = 0;
                    break;
                case WaveData.EnemyType.RangedStandard: 
                    index = 1; 
                    break;
            }

            GameObject spawned = enemyPools[index].Get().gameObject;
            spawned.GetComponent<MeleeEnemy>().Initialize();
            spawned.transform.position = position;
        }

        public void SpawnAll()
        {
            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
            {
                spawnPoint.SpawnQueuedEnemies();
            }
        }

        public void ReturnToPool(WaveData.EnemyType enemy, Transform item)
        {
            int index = 0;
            switch (enemy)
            {
                case WaveData.EnemyType.MeleeStandard:
                    index = 0;
                    break;
                case WaveData.EnemyType.RangedStandard:
                    index = 1;
                    break;
            }

            if (!enemyPools[index].Return(item))
            {
                Debug.LogError("Could not return enemy to pool!");
            }
        }
    }
}
