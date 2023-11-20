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

        public string PromptText
        {
            get { return promptText; }
        }

        private bool activated = false;

        private void LoadTarget()
        {
            FadeCanvas fadeCanvas = GameInstance.Instance.GetFadeCanvas();
            fadeCanvas.OnFadeToComplete -= LoadTarget;
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
            fadeCanvas.OnFadeToComplete += LoadTarget;
            activated = true;
            return true;
        }

        public void OnHover(bool state)
        {
        }
    }
}
