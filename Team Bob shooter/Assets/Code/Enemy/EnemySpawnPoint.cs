using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Transform[] points;

        [SerializeField]
        private float spawnRate = 0.25f;

        private List<WaveData.WaveEnemy> spawnQueue = new List<WaveData.WaveEnemy>();

        private EnemySpawning enemySpawning;

        private Coroutine spawnRoutine;

        private void Awake()
        {
            enemySpawning = FindObjectOfType<EnemySpawning>();
        }

        public void QueueEnemy(WaveData.WaveEnemy enemy)
        {
            spawnQueue.Add(enemy);
        }

        public void SpawnQueuedEnemies()
        {
            spawnRoutine = StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            int index = 0;
            while (spawnQueue.Count > 0)
            {
                enemySpawning.SpawnEnemy(spawnQueue[0], points[index].position);
                spawnQueue.RemoveAt(0);
                index++;
                if (index == points.Length) index = 0;

                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}
