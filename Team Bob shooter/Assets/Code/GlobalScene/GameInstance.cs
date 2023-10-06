using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TeamBobFPS;
using TeamBobFPS.UI;

namespace TeamBobFPS
{
    public class GameInstance : MonoBehaviour
    {
        [SerializeField]
        private PlayerDefeatedCanvas playerDefeatedCanvas;

        [SerializeField]
        private InGameHudCanvas inGameHudCanvas;

        [SerializeField]
        private UpdateManager updateManager;

        private static GameInstance instance;

        private GameStateManager gameStateManager;

        private WeaponLoadout weaponLoadout;

        private MapAreaManager mapAreaManager;

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
            weaponLoadout = GetComponent<WeaponLoadout>();
            AudioListener = GetComponent<AudioListener>();
            mapAreaManager = GetComponent<MapAreaManager>();
            UsingController = Gamepad.all.Count > 0;
            //Application.targetFrameRate = 60;
        }

        private void Update()
        {
            //audioManager.NextFrame();
        }

        // Getters

        public PlayerDefeatedCanvas GetPlayerDefeatedCanvas() { return  playerDefeatedCanvas; }

        public InGameHudCanvas GetInGameHudCanvas() { return inGameHudCanvas; }

        public UpdateManager GetUpdateManager() { return updateManager; }

        public GameStateManager GetGameStateManager() { return gameStateManager; }

        public WeaponLoadout GetWeaponLoadout() { return weaponLoadout; }

        public MapAreaManager GetMapAreaManager() {  return mapAreaManager; }
    }
}

