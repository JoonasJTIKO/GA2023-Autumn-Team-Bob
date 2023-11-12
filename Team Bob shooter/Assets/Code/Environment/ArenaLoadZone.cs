using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using UnityEngine;

namespace TeamBobFPS
{
    public class ArenaLoadZone : MonoBehaviour
    {
        [SerializeField]
        private StateType targetState;

        private void OnTriggerEnter(Collider other)
        {
            FadeCanvas fadeCanvas = GameInstance.Instance.GetFadeCanvas();
            fadeCanvas.FadeTo(0.5f);
            fadeCanvas.OnFadeToComplete += LoadTarget;
        }

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
    }
}
