using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public abstract class WaveClearAction : MonoBehaviour
    {
        [SerializeField]
        protected int waveIndex = 0;

        protected bool activated = false;

        protected virtual void OnEnable()
        {
            WaveManager.OnWaveCleared += Activate;
        }

        protected virtual void OnDisable()
        {
            WaveManager.OnWaveCleared -= Activate;
        }

        private void Activate(int waveIndex, int levelIndex)
        {
            DoAction(waveIndex, levelIndex);
        }

        protected virtual bool DoAction(int waveIndex, int levelIndex)
        {
            if (waveIndex != this.waveIndex || activated) return false;

            activated = true;
            return true;
        }
    }
}
