using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS
{
    public class TextWriteOverTime : MonoBehaviour
    {
        [SerializeField]
        private float characterDelay = 0.1f;

        private Coroutine coroutine;

        private string targetText;

        private char[] chars;

        private TMP_Text textComponent;

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
            targetText = textComponent.text;
            chars = targetText.ToCharArray();
        }

        public void StartWrite(string text = "")
        {
            if (textComponent == null || GameInstance.Instance == null) return;

            textComponent.text = "";
            targetText = text;
            chars = targetText.ToCharArray();
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Write());
        }

        private IEnumerator Write()
        {
            int index = 0;
            float timer = 0f;

            while (index < chars.Length)
            {
                if (timer >= characterDelay)
                {
                    textComponent.text += chars[index];
                    index++;
                    timer = 0f;
                }
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }
        }
    }
}
