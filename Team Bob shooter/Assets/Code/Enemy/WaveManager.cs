using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamBobFPS
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField]
        private WaveData[] waves;

        [SerializeField]
        private ArenaLoadZone levelExit;

        private int waveIndex = 0;

        private WaveData currentWave;

        private Dictionary<WaveData.WaveEnemy, int> currentWaveEnemies = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> maxAmountInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> reinforcementInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> defeatedCountInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> totalAmountInfo = new Dictionary<WaveData.WaveEnemy, int>();

        private EnemySpawning enemySpawning;

        private void Start()
        {
            enemySpawning = GetComponent<EnemySpawning>();
            currentWave = waves[waveIndex];
            StartWave(currentWave);
        }

        private void OnEnable()
        {
            MeleeEnemy.OnDefeated += EnemyDefeated;
        }
        private void OnDisable()
        {
            MeleeEnemy.OnDefeated -= EnemyDefeated;
        }

        private void StartWave(WaveData wave)
        {
            currentWaveEnemies.Clear();
            maxAmountInfo.Clear();
            reinforcementInfo.Clear();
            defeatedCountInfo.Clear();
            totalAmountInfo.Clear();

            //Spawn initial enemies
            foreach (WaveData.WaveEnemy enemy in wave.Enemies)
            {
                if (enemy.SpawnPoints.Length > 0)
                {
                    enemySpawning.SpawnEnemies(enemy, enemy.MaxConcurrent, enemy.SpawnPoints);
                }
                else
                {
                    enemySpawning.SpawnEnemies(enemy, enemy.MaxConcurrent);
                }

                currentWaveEnemies.Add(enemy, enemy.MaxConcurrent);
                maxAmountInfo.Add(enemy, enemy.MaxConcurrent);
                reinforcementInfo.Add(enemy, enemy.ReinforcementThreshold);
                totalAmountInfo.Add(enemy, enemy.TotalAmount);
            }
            enemySpawning.SpawnAll();


            //Notify of wave start
        }

        public void EnemyDefeated(WaveData.EnemyType enemyType, Transform item)
        {
            enemySpawning.ReturnToPool(enemyType, item);

            WaveData.WaveEnemy enemy = null;
            switch (enemyType)
            {
                case WaveData.EnemyType.MeleeStandard:
                    enemy = currentWaveEnemies.ElementAt(0).Key;
                    break;
                case WaveData.EnemyType.RangedStandard:
                    enemy = currentWaveEnemies.ElementAt(1).Key;
                    break;
            }

            currentWaveEnemies[enemy]--;
            if (defeatedCountInfo.ContainsKey(enemy))
            {
                defeatedCountInfo[enemy]++;
            }
            else
            {
                defeatedCountInfo.Add(enemy, 1);
            }


            int maxSpawn = totalAmountInfo[enemy] - defeatedCountInfo[enemy];

            if (currentWaveEnemies[enemy] <= reinforcementInfo[enemy])
            {
                int amountToSpawn = maxAmountInfo[enemy] - reinforcementInfo[enemy];
                if (amountToSpawn > maxSpawn)
                {
                    amountToSpawn = maxSpawn;
                }

                if (amountToSpawn > 0)
                {
                    if (enemy.SpawnPoints.Length > 0)
                    {
                        enemySpawning.SpawnEnemies(enemy, amountToSpawn, enemy.SpawnPoints);
                    }
                    else
                    {
                        enemySpawning.SpawnEnemies(enemy, amountToSpawn);
                    }
                    enemySpawning.SpawnAll();
                    currentWaveEnemies[enemy] += amountToSpawn;
                }
            }

            foreach (int count in currentWaveEnemies.Values)
            {
                if (count > 0) return;
            }
            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                levelExit.gameObject.SetActive(true);
                return;
            }
            currentWave = waves[waveIndex];
            StartWave(currentWave);
        }
    }
}