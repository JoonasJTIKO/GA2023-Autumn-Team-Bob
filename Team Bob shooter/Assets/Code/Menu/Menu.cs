using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class Menu : MenuCanvas
    {
        public void Continue()
        {
            GameInstance.Instance.GetPauseMenu().Hide();
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(2);
        }

        public void Quit()
        {
            Application.Quit();
            Debug.Log("Quit");
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Restart");
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
