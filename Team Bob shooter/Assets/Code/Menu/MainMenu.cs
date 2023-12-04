using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class MainMenu : MonoBehaviour
    {
        public Canvas mainCanvas;
        public Canvas settingsCanvas;

        private void Awake()
        {
            settingsCanvas.gameObject.SetActive(false);
        }

        public void OpenOptions()
        {
            settingsCanvas.gameObject.SetActive(true);
            mainCanvas.gameObject.SetActive(false);
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

        public void Close()
        {
            settingsCanvas.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
        }
    }
}
