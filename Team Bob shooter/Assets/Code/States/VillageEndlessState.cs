using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class VillageEndlessState : GameStateBase
    {
        public override string SceneName { get { return "VillageEndless"; } }

        public override StateType Type { get { return StateType.Arena1Endless; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }

            GameInstance.Instance.GetLevelSelectCanvas().Hide();
            GameInstance.Instance.GetInGameHudCanvas().Show();
            GameInstance.Instance.GetInGameHudCanvas().ActivateWaveInfo(true);
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
            GameInstance.Instance.GetInGameHudCanvas().Hide();
        }

        public VillageEndlessState() : base() 
        {
            AddTargetState(StateType.MainMenu);
            AddTargetState(StateType.Hub);
        }
    }
}
