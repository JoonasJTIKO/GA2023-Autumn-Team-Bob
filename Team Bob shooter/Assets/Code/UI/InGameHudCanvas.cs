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

        [SerializeField]
        private TMP_Text healthText;

        [SerializeField]
        private AnimationCurve healthShakeCurve;

        private float currentHealthValue = 100;

        private int currentWaveNumber = 1;

        [SerializeField]
        private string[] newWaveTexts;

        [SerializeField]
        private Animator WaveClearAnimator;

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
            GetComponentInChildren<ObjectiveText>(true).gameObject.SetActive(!state);
        }

        public void WaveCleared()
        {
            WaveClearAnimator.SetTrigger("PlayWipe");
        }

        public void UpdateWaveNumber()
        {
            currentWaveNumber++;

            waveNumber.text = currentWaveNumber.ToString();

            //newWaveTestText.text = newWaveTexts[waveIndex];
            //StartCoroutine(TextFade());
        }

        public void ReduceHealth(float amount)
        {
            currentHealthValue -= amount;
            currentHealthValue = Mathf.Clamp(currentHealthValue, 0, 100);
            healthText.text = currentHealthValue.ToString();
            StartCoroutine(HealthTextShake());
        }

        public void AddHealth(float amount)
        {
            currentHealthValue += amount;
            currentHealthValue = Mathf.Clamp(currentHealthValue, 0, 100);
            healthText.text = currentHealthValue.ToString();
        }

        public override void Show()
        {
            base.Show();

            if (interactText != null)
            {
                interactText.text = "";
            }
        }

        private IEnumerator HealthTextShake()
        {
            RectTransform rectTransform = healthText.GetComponent<RectTransform>();
            Vector3 startPos = rectTransform.localPosition;
            float timer = 0f;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                float shakeStrength = healthShakeCurve.Evaluate(timer / 0.5f) * 0.1f;
                rectTransform.localPosition = startPos + Random.insideUnitSphere * shakeStrength;
                yield return null;
            }

            rectTransform.localPosition = startPos;
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
