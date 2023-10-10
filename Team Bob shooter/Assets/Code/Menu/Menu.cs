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
        private PlayerInputs playerInputs;
        private InputAction menu;

        [SerializeField] private GameObject pauseUI;
        private bool isPaused;
        InGameState state;

        protected override void Awake()
        {
            playerInputs = new PlayerInputs();
            isPaused = false;
            pauseUI.SetActive(false);
        }
        public void Continue()
        {
            DeActivateMenu();
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

        protected override void OnEnable()
        {
            menu = playerInputs.Menu.Pause;
            menu.Enable();

            menu.performed += Pause;
        }

        protected override void OnDisable()
        {
            menu.Disable();
        }

        public void Pause(InputAction.CallbackContext context)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                ActivateMenu();
            }
            else
            {
                DeActivateMenu();
            }
        }

        void ActivateMenu()
        {
            Time.timeScale = 0;
            pauseUI.SetActive(true);
            isPaused = true;
        }
        void DeActivateMenu()
        {
            Time.timeScale = 1;
            pauseUI.SetActive(false);
            isPaused = false;
        }
    }
}
