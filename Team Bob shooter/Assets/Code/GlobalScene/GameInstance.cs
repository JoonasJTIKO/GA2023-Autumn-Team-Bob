using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TeamBobFPS;

namespace TeamBobFPS
{
    public class GameInstance : MonoBehaviour
    {
        [SerializeField]
        private UpdateManager updateManager;

        private static GameInstance instance;

        private GameStateManager gameStateManager;

        public AudioListener AudioListener
        {
            get;
            private set;
        }

        public bool UsingController
        {
            get;
            private set;
        }

        public bool LoadExistingSave
        {
            get; set;
        }

        public static GameInstance Instance
        {
            get
            {
                if (!instance)
                {
                    instance = GameObject.FindObjectOfType<GameInstance>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = GetComponent<GameInstance>();
            }
            gameStateManager = GetComponent<GameStateManager>();
            AudioListener = GetComponent<AudioListener>();
            UsingController = Gamepad.all.Count > 0;
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            //audioManager.NextFrame();
        }

        // Getters

        public UpdateManager GetUpdateManager() { return updateManager; }

        public GameStateManager GetGameStateManager() { return gameStateManager; }
    }
}

