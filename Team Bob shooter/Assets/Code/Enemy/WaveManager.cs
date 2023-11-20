using System;
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

        [SerializeField]
        private int levelIndex = 1;

        [SerializeField]
        private bool endless = false;

        private bool backwards = false;

        private int endlessLoop = 0;

        private int waveIndex = 0;

        private WaveData currentWave;

        private Dictionary<WaveData.WaveEnemy, int> currentWaveEnemies = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> maxAmountInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> reinforcementInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> defeatedCountInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, int> totalAmountInfo = new Dictionary<WaveData.WaveEnemy, int>();
        private Dictionary<WaveData.WaveEnemy, bool> canSpawnReinforcements = new Dictionary<WaveData.WaveEnemy, bool>(); 

        private EnemySpawning enemySpawning;

        private CutsceneManager cutsceneManager;

        public static event Action<int, int> OnWaveCleared;

        public static event Action<int> OnLevelCleared;

        private void Start()
        {
            enemySpawning = GetComponent<EnemySpawning>();
            cutsceneManager = FindObjectOfType<CutsceneManager>();
            currentWave = waves[waveIndex];
            OnWaveCleared?.Invoke(waveIndex - 1, levelIndex);
            StartCoroutine(StartFirstWave());
        }

        private void OnEnable()
        {
            MeleeEnemy.OnDefeated += EnemyDefeated;
            RangeEnemy.OnDefeated += EnemyDefeated;
            FlyingEnemy.OnDefeated += EnemyDefeated;
        }
        private void OnDisable()
        {
            MeleeEnemy.OnDefeated -= EnemyDefeated;
            RangeEnemy.OnDefeated -= EnemyDefeated;
            FlyingEnemy.OnDefeated -= EnemyDefeated;
        }

        /// <summary>
        /// Starts a new wave, updating dictionaries and beginning spawning
        /// </summary>
        /// <param name="wave">Wave object with all wave details</param>
        public void StartWave(WaveData wave)
        {
            currentWaveEnemies.Clear();
            maxAmountInfo.Clear();
            reinforcementInfo.Clear();
            defeatedCountInfo.Clear();
            totalAmountInfo.Clear();
            canSpawnReinforcements.Clear();

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
                if (endlessLoop == 0)
                {
                    totalAmountInfo.Add(enemy, enemy.TotalAmount);
                }
                else
                {
                    float multiplier = Mathf.Clamp(1f + (0.5f * endlessLoop), 1.5f, 3f);
                    totalAmountInfo.Add(enemy, (int)(enemy.TotalAmount * multiplier));
                }
                canSpawnReinforcements.Add(enemy, enemy.MaxConcurrent != totalAmountInfo[enemy]);
            }
            enemySpawning.SpawnRate = wave.SpawnRate;
            enemySpawning.SpawnAll();


            //Notify of wave start
        }

        /// <summary>
        /// Updates dictionaries, checks if more enemies shall be spawned / if wave has been cleared
        /// </summary>
        /// <param name="enemyType">Type of enemy defeated</param>
        /// <param name="item">The enemy</param>
        public void EnemyDefeated(WaveData.EnemyType enemyType, Transform item)
        {
            StartCoroutine(enemySpawning.ReturnToPool(enemyType, item));

            WaveData.WaveEnemy enemy = null;
            switch (enemyType)
            {
                case WaveData.EnemyType.MeleeStandard:
                    enemy = currentWaveEnemies.ElementAt(0).Key;
                    break;
                case WaveData.EnemyType.RangedStandard:
                    enemy = currentWaveEnemies.ElementAt(1).Key;
                    break;
                case WaveData.EnemyType.Flying:
                    enemy = currentWaveEnemies.ElementAt(2).Key;
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


            int maxSpawn = totalAmountInfo[enemy] - defeatedCountInfo[enemy] - currentWaveEnemies[enemy];

            if (currentWaveEnemies[enemy] <= reinforcementInfo[enemy] && canSpawnReinforcements[enemy])
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
            OnWaveCleared?.Invoke(waveIndex, levelIndex);
            cutsceneManager.PlayCutscene(waveIndex);
            if (backwards)
            {
                waveIndex--;
                if (waveIndex < 0)
                {
                    backwards = false;
                    waveIndex++;
                }
            }
            else
            {
                waveIndex++;
                if (waveIndex >= waves.Length)
                {
                    if (endless)
                    {
                        backwards = true;
                        waveIndex -= 2;
                        endlessLoop++;
                    }
                    else
                    {
                        AllWavesCleared();
                        return;
                    }
                }
            }
            
            currentWave = waves[waveIndex];
            StartWave(currentWave);
        }

        /// <summary>
        /// Activates level exit, invokes event for other things to react
        /// </summary>
        private void AllWavesCleared()
        {
            levelExit.gameObject.SetActive(true);
            GameInstance.Instance.GetGameProgressionManager().UpdateGameProgress(levelIndex);
            OnLevelCleared?.Invoke(1);
        }

        private IEnumerator StartFirstWave()
        {
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            StartWave(currentWave);
        }
    }
}
