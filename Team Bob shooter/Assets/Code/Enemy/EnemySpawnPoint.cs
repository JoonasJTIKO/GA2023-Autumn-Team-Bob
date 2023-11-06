using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Transform[] points;

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

        public void SpawnQueuedEnemies(float spawnRate)
        {
            spawnRoutine = StartCoroutine(Spawn(spawnRate));
        }

        private IEnumerator Spawn(float spawnRate)
        {
            int index = 0;
            while (spawnQueue.Count > 0)
            {
                enemySpawning.SpawnEnemy(spawnQueue[0], points[index].position);
                spawnQueue.RemoveAt(0);
                index++;
                if (index == points.Length) index = 0;

                float timer = 0;
                while (timer < spawnRate)
                {
                    timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                    yield return null;
                }
            }
        }
    }
}
