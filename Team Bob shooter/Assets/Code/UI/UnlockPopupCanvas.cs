using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class UnlockPopupCanvas : MenuCanvas
    {
        private TMP_Text unlockText;

        private List<string> queuedUnlockTexts = new List<string>();

        private Animator animator;

        public event Action UnlocksPlayed;

        protected override void Awake()
        {
            base.Awake();

            unlockText = GetComponentInChildren<TMP_Text>();
            animator = GetComponentInChildren<Animator>();
        }

        public void QueueUnlockAnimation(string text)
        {
            queuedUnlockTexts.Add(text);
        }

        public IEnumerator PlayQueuedUnlocks()
        {
            bool textChanged = false;
            float timeElapsed = 0f;
            while (queuedUnlockTexts.Count > 0)
            {
                if (!textChanged)
                {
                    unlockText.text = queuedUnlockTexts[0];
                    textChanged = true;
                    animator.SetTrigger("Play");
                }

                timeElapsed += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                if (timeElapsed >= 2.5f)
                {
                    textChanged = false;
                    timeElapsed = 0f;
                    queuedUnlockTexts.RemoveAt(0);
                }
                yield return null;
            }
            UnlocksPlayed?.Invoke();
        }
    }
}
