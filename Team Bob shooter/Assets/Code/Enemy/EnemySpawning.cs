using System;
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

        public float SpawnRate;

        private void Awake()
        {
            int i = 0;
            foreach (Transform enemyPrefab in enemyPrefabs)
            {
                enemyPools[i] = new ComponentPool<Transform>(enemyPrefab.transform, 20);
                i++;
            }
        }

        public void SpawnEnemies(WaveData.WaveEnemy enemy, int amount, EnemySpawnPoint[] specifiedSpawnPoints = null)
        {
            if (amount <= 0) return;

            EnemySpawnPoint[] possibleSpawnPoints = null;
            if (specifiedSpawnPoints != null)
            {
                possibleSpawnPoints = specifiedSpawnPoints;
            }
            else
            {
                possibleSpawnPoints = spawnPoints;
            }

            if (amount / enemy.SpawnGroupSize > possibleSpawnPoints.Length)
            {
                Debug.LogError("Can not spawn more enemy groups than there are spawn points!");
                return;
            }

            int unallocatedAmount = amount;

            List<EnemySpawnPoint> selectedPoints = new List<EnemySpawnPoint>();

            while (unallocatedAmount > 0)
            {
                EnemySpawnPoint spawnPoint = possibleSpawnPoints[UnityEngine.Random.Range(0, possibleSpawnPoints.Length)];
                if (!selectedPoints.Contains(spawnPoint))
                {
                    selectedPoints.Add(spawnPoint);

                    int amountToSpawn = enemy.SpawnGroupSize;
                    if (unallocatedAmount - enemy.SpawnGroupSize < 0)
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
            GameObject spawned = null;
            switch (enemy.EnemyType)
            {
                case WaveData.EnemyType.MeleeStandard:
                    spawned = enemyPools[0].Get().gameObject;
                    spawned.transform.position = position;
                    spawned.GetComponent<MeleeEnemy>().Initialize();
                    break;
                case WaveData.EnemyType.RangedStandard:
                    spawned = enemyPools[1].Get().gameObject;
                    spawned.transform.position = position;
                    spawned.GetComponent<RangeEnemy>().Initialize();
                    break;
                case WaveData.EnemyType.Flying:
                    spawned = enemyPools[2].Get().gameObject;
                    spawned.transform.position = position;
                    spawned.GetComponent<FlyingEnemy>().Initialize();
                    break;
            }
        }

        public void SpawnAll()
        {
            foreach (EnemySpawnPoint spawnPoint in spawnPoints)
            {
                spawnPoint.SpawnQueuedEnemies(SpawnRate);
            }
        }

        public IEnumerator ReturnToPool(WaveData.EnemyType enemy, Transform item)
        {
            yield return null;

            int index = 0;
            switch (enemy)
            {
                case WaveData.EnemyType.MeleeStandard:
                    index = 0;
                    break;
                case WaveData.EnemyType.RangedStandard:
                    index = 1;
                    break;
                case WaveData.EnemyType.Flying: 
                    index = 2; 
                    break;
            }

            if (!enemyPools[index].Return(item))
            {
                Debug.LogError("Could not return enemy to pool!");
            }
        }
    }
}
