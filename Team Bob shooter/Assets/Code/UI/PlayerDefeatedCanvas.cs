using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class PlayerDefeatedCanvas : MenuCanvas
    {
        public void RetryStage()
        {
            if (buttonPressed) return;

            buttonPressed = true;
            GameInstance.Instance.GetGameStateManager().ReloadCurrentState();
        }

        public void BackToHub()
        {
            if (buttonPressed) return;

            buttonPressed = true;
            GameInstance.Instance.GetGameStateManager().Go(StateType.MainMenu);
        }
    }
}
