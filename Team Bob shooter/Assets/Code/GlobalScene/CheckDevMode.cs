using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamBobFPS
{
    public class CheckDevMode : MonoBehaviour
    {
        public string SceneName
        {
            get;
            private set;
        }

        private void Awake()
        {
            if (Application.isEditor && SceneManager.sceneCount == 1)
            {
                DontDestroyOnLoad(gameObject);
                Scene scene = SceneManager.GetActiveScene();
                SceneName = scene.name;
                SceneManager.UnloadSceneAsync(scene);
                SceneManager.LoadScene(0);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
