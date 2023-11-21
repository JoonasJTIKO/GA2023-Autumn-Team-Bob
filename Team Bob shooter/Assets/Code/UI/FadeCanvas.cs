using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS.UI
{
    public class FadeCanvas : MenuCanvas
    {
        private Image image;

        private float fadeTime = 1f;

        private Coroutine activeRoutine;

        public event Action OnFadeToComplete;

        public event Action OnFadeFromComplete;

        private void Start()
        {
            image = GetComponentInChildren<Image>();
        }

        public void FadeTo(float fadeTime)
        {
            this.fadeTime = fadeTime;

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = StartCoroutine(FadeTo());
        }

        public void FadeFrom(float fadeTime)
        {
            if (image == null) return;

            this.fadeTime = fadeTime;

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = StartCoroutine(FadeFrom());
        }

        private IEnumerator FadeTo()
        {
            float currentAlpha = image.color.a;
            float timer = 0;
            Debug.Log("start alpha: " + currentAlpha);
            while (currentAlpha < 1)
            {
                currentAlpha = Mathf.Lerp(0, 1, timer / fadeTime);
                Color newColor = image.color;
                newColor.a = currentAlpha;
                image.color = newColor;
                Debug.Log(image.color.a);

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            OnFadeToComplete?.Invoke();
        }

        private IEnumerator FadeFrom()
        {
            float currentAlpha = image.color.a;
            float timer = 0;

            while (currentAlpha > 0)
            {
                currentAlpha = Mathf.Lerp(1, 0, timer / fadeTime);
                Color newColor = image.color;
                newColor.a = currentAlpha;
                image.color = newColor;

                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
            OnFadeFromComplete?.Invoke();
        }
    }
}
