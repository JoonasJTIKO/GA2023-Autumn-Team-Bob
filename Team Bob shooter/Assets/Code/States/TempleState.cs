using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class TempleState : GameStateBase
    {
        public override string SceneName { get { return "Temple"; } }

        public override StateType Type { get { return StateType.Arena2; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetLevelSelectCanvas().Hide();
            GameInstance.Instance.GetPlayerDefeatedCanvas().Hide();
            GameInstance.Instance.GetInGameHudCanvas().Show();
            GameInstance.Instance.GetInGameHudCanvas().ActivateWaveInfo(false);
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetInGameHudCanvas().Hide();
        }

        public TempleState() : base() 
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Hub);
        }
    }
}
