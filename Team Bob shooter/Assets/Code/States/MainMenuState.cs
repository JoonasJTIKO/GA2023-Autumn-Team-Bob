using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class MainMenuState : GameStateBase
    {
        public override string SceneName { get { return "MainMenu"; } }

        public override StateType Type { get { return StateType.MainMenu; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetInGameHudCanvas().Show();
            GameInstance.Instance.AudioListener.enabled = true;
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetInGameHudCanvas().Hide();

            GameInstance.Instance.AudioListener.enabled = false;
        }

        public MainMenuState() : base()
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Arena1);
        }
    }
}
