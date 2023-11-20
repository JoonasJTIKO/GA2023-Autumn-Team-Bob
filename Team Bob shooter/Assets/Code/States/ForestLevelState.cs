using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class ForestLevelState : GameStateBase
    {
        public override string SceneName { get { return "lvl1"; } }

        public override StateType Type { get { return StateType.Arena2; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetLevelSelectCanvas().Hide();
            GameInstance.Instance.GetInGameHudCanvas().Show();
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetInGameHudCanvas().Hide();
        }

        public ForestLevelState() : base() 
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Hub);
        }
    }
}
