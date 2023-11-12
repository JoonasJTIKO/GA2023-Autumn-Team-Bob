using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class HubState : GameStateBase
    {
        public override string SceneName { get { return "HubScene"; } }

        public override StateType Type { get { return StateType.Hub; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetInGameHudCanvas().Show();
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetInGameHudCanvas().Hide();
        }

        public HubState() : base()
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Arena1);
            AddTargetState(StateType.Arena2);
        }
    }
}
