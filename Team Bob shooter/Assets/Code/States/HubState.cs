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

            GameInstance.Instance.GetPlayerDefeatedCanvas().Hide();
            GameInstance.Instance.GetLoadoutSelectCanvas().Show();
            GameInstance.Instance.GetFadeCanvas().FadeFrom(0.5f);

            GameInstance.Instance.AudioListener.enabled = true;
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetLoadoutSelectCanvas().Hide();

            GameInstance.Instance.AudioListener.enabled = false;
        }

        public HubState() : base()
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Arena1);
            AddTargetState(StateType.Arena2);
            AddTargetState(StateType.Arena1Endless);
            AddTargetState(StateType.Arena2Endless);
        }
    }
}
