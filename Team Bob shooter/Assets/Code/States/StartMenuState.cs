using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class StartMenuState : GameStateBase
    {
        public override string SceneName { get { return "PlayerTest"; } }

        public override StateType Type { get { return StateType.MainMenu; } }

        public override void Activate(bool loadScene = true)
        {
            if (SceneManager.GetActiveScene().name.ToLower() != SceneName.ToLower() && loadScene)
            {
                SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            }
        }

        public override void Deactivate(bool unloadScene = true)
        {
            if (unloadScene) SceneManager.UnloadSceneAsync(SceneName);
        }

        public StartMenuState() : base() 
        {

        }
    }
}
