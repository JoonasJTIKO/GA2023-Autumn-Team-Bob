using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class PauseMenuController : MenuCanvas
    {
        PlayerUnit playerUnit;

        protected override void Awake()
        {
            base.Awake();
            playerUnit = FindAnyObjectByType<PlayerUnit>();
        }

        public void Continue()
        {
            if (PlayerUnit.isPaused)
            {
                GameInstance.Instance.GetUpdateManager().timeScale = 1;
                GameInstance.Instance.GetUpdateManager().fixedTimeScale = 1;
                GameInstance.Instance.GetPauseMenu().Hide();
                PlayerUnit.isPaused = false;
                playerUnit.PauseConstraints();
            }
        }

        public void Restart()
        {
            Hide();
            GameInstance.Instance.GetUpdateManager().timeScale = 1;
            GameInstance.Instance.GetUpdateManager().fixedTimeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Restart");
        }

        public void BackToMenu()
        {
            Hide();
            GameInstance.Instance.GetUpdateManager().timeScale = 1;
            GameInstance.Instance.GetUpdateManager().fixedTimeScale = 1;
            GameInstance.Instance.GetGameStateManager().Go(StateType.MainMenu);
        }
    }
}
