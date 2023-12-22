using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using UnityEngine;

namespace TeamBobFPS
{
    public class ArenaLoadZone : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private StateType targetState;

        [SerializeField]
        private string promptText;

        [SerializeField]
        private string[] unlockTexts;

        public string PromptText
        {
            get { return promptText; }
        }

        private bool activated = false;

        private void DisplayUnlocks()
        {
            GameInstance.Instance.GetFadeCanvas().OnFadeToComplete -= DisplayUnlocks;
            if (unlockTexts.Length == 0) LoadTarget();
            else
            {
                foreach (string text in unlockTexts)
                {
                    GameInstance.Instance.GetUnlockPopupCanvas().QueueUnlockAnimation(text);
                }
                StartCoroutine(GameInstance.Instance.GetUnlockPopupCanvas().PlayQueuedUnlocks());
                GameInstance.Instance.GetUnlockPopupCanvas().UnlocksPlayed += LoadTarget;
            }
        }

        private void LoadTarget()
        {
            FadeCanvas fadeCanvas = GameInstance.Instance.GetFadeCanvas();
            GameInstance.Instance.GetUnlockPopupCanvas().UnlocksPlayed -= LoadTarget;
            GameInstance.Instance.GetGameStateManager().Go(targetState);
        }

        public void SetTargetState(StateType targetState)
        {
            this.targetState = targetState;
        }

        public bool OnInteract(int currentWeapon)
        {
            if (activated) return false;

            FadeCanvas fadeCanvas = GameInstance.Instance.GetFadeCanvas();
            fadeCanvas.FadeTo(0.5f);
            fadeCanvas.OnFadeToComplete += DisplayUnlocks;
            activated = true;
            return true;
        }

        public void OnHover(bool state)
        {
        }
    }
}
