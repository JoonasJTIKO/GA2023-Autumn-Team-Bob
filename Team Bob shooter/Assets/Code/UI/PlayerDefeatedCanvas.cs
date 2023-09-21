using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS.UI
{
    public class PlayerDefeatedCanvas : MenuCanvas
    {
        public void RetryStage()
        {
            GameInstance.Instance.GetGameStateManager().ReloadCurrentState();
            Hide();
        }

        public void BackToHub()
        {
            GameInstance.Instance.GetGameStateManager().Go(StateType.Hub);
            Hide();
        }
    }
}
