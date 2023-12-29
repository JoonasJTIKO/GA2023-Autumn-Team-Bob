using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class MainMenuState : GameStateBase
    {
        public override string SceneName { get { return "MainMenuBackground"; } }

        public override StateType Type { get { return StateType.MainMenu; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetInGameHudCanvas().Hide();
            GameInstance.Instance.GetMainMenu().Show();
            GameInstance.Instance.GetFadeCanvas().FadeFrom(1f);
            GameInstance.Instance.AudioListener.enabled = true;

            GameInstance.Instance.GetAudioManager().PlayMusic(EGameMusic._MENU_MUSIC);
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetMainMenu().Hide();

            GameInstance.Instance.AudioListener.enabled = false;
            GameInstance.Instance.GetAudioManager().FadeMusicOut(0.5f, false);
        }

        public MainMenuState() : base()
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Hub);
            AddTargetState(StateType.Arena1);
            AddTargetState(StateType.Arena2);
            AddTargetState(StateType.Arena1Endless);
            AddTargetState(StateType.Arena2Endless);
        }
    }
}
