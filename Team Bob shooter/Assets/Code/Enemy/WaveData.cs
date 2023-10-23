using UnityEngine;

namespace TeamBobFPS
{
    [System.Serializable]
    public class WaveData
    {
        public enum EnemyType
        {
            MeleeStandard,
            RangedStandard
        }

        [System.Serializable]
        public class WaveEnemy
        {
            public EnemyType EnemyType;

            public int TotalAmount;

            public int MaxConcurrent;

            public int ReinforcementThreshold;

            public int SpawnGroupSize;

            public EnemySpawnPoint[] SpawnPoints;
        }

        public WaveEnemy[] Enemies;

        public float SpawnRate = 0.25f;
    }
}
