using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class InGameHudCanvas : MenuCanvas
    {
        [SerializeField]
        private TMP_Text magCountText;

        [SerializeField]
        private TMP_Text reserveCountText;

        [SerializeField]
        private TMP_Text interactText;

        [SerializeField]
        private TMP_Text newWaveTestText;

        [SerializeField]
        private TMP_Text waveNumber;

        private int currentWaveNumber = 1;

        [SerializeField]
        private string[] newWaveTexts;

        protected override void OnEnable()
        {
            base.OnEnable();

            WaveManager.OnWaveCleared += ActivateNewWaveTestText;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            WaveManager.OnWaveCleared -= ActivateNewWaveTestText;
        }

        public void UpdateMagCount(int amount)
        {
            magCountText.text = amount.ToString();
        }

        public void UpdateReserveCount(int amount)
        {
            reserveCountText.text = amount.ToString();
        }

        public void SetInteractText(string text)
        {
            interactText.text = text;
        }

        public void ActivateWaveInfo(bool state)
        {
            waveNumber.transform.parent.gameObject.SetActive(state);
            GetComponentInChildren<ObjectiveText>().gameObject.SetActive(!state);
        }

        private void ActivateNewWaveTestText(int waveIndex, int levelIndex)
        {
            if (waveIndex < 0) return;

            currentWaveNumber++;
            waveNumber.text = currentWaveNumber.ToString();

            //newWaveTestText.text = newWaveTexts[waveIndex];
            //StartCoroutine(TextFade());
        }

        private IEnumerator TextFade()
        {
            newWaveTestText.enabled = true;

            Color32 textColor = newWaveTestText.color;

            float fadeTime = 1f;
            float timer = fadeTime;
            float alpha = 0f;
            while (timer >= 0f)
            {
                alpha = 100 - (timer / fadeTime) * 100;
                textColor.a = (byte)alpha;
                newWaveTestText.color = textColor;
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            textColor.a = 100;
            newWaveTestText.color = textColor;

            timer = 2f;
            while (timer >= 0f)
            {
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            timer = fadeTime;
            while (timer >= 0f)
            {
                alpha = (timer / fadeTime) * 100;
                textColor.a = (byte)alpha;
                newWaveTestText.color = textColor;
                timer -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            newWaveTestText.enabled = false;
        }
    }
}
