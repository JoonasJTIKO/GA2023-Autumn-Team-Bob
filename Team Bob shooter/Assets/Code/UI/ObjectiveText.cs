using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS
{
    public class ObjectiveText : MonoBehaviour
    {
        [SerializeField]
        private string[] objectiveTexts;

        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            WaveManager.OnWaveCleared += DoAction;
        }

        private void OnDisable()
        {
            WaveManager.OnWaveCleared -= DoAction;
        }

        private void DoAction(int waveIndex, int levelIndex)
        {
            int index = waveIndex + 1;

            switch (levelIndex)
            {
                case 0:
                    break;
                case 1:
                    index += 3;
                    break;
                case 2:
                    index += 8;
                    break;
            }

            text.text = objectiveTexts[index];
        }
    }
}
