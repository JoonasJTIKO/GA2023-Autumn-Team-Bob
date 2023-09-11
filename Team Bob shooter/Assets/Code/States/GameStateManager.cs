using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class GameStateManager : MonoBehaviour
    {
        [SerializeField]
        private StateType initialState;

        private List<GameStateBase> states = new List<GameStateBase>();

        public GameStateBase CurrentState
        {
            get;
            private set;
        }

        public GameStateBase PreviousState
        {
            get;
            private set;
        }

        private void Start()
        {
            Initialize();

            if (Application.isEditor)
            {
                GameObject devModeObject = GameObject.Find("DevMode");
                if (devModeObject != null)
                {
                    CheckDevMode checkDevMode = devModeObject.GetComponent<CheckDevMode>();
                    foreach (GameStateBase state in states)
                    {
                        if (state.SceneName == checkDevMode.SceneName)
                        {
                            Go(state.Type);
                            break;
                        }
                    }
                    Destroy(devModeObject);
                    return;
                }
            }

            LoadInitialState();
        }

        private void Initialize()
        {
            InGameState inGameState = new InGameState();
            HubState hubState = new HubState();

            states.Add(inGameState);
            states.Add(hubState);
        }

        private void LoadInitialState()
        {
            foreach (GameStateBase state in states)
            {
                if (state.Type == initialState)
                {
                    CurrentState = state;
                    CurrentState.Activate();
                    break;
                }
            }
        }

        private GameStateBase GetState(StateType type)
        {
            foreach (GameStateBase state in states)
            {
                if (state.Type == type)
                {
                    return state;
                }
            }

            return null;
        }

        public bool Go(StateType targetStateType, bool unloadCurrent = true, bool loadScene = true)
        {
            if (CurrentState != null && !CurrentState.IsValidTarget(targetStateType))
            {
                Debug.LogWarning("Cannot transition to target state because it is not a valid target");
                return false;
            }

            GameStateBase nextState = GetState(targetStateType);
            if (nextState == null)
            {
                Debug.LogWarning("Target state does not exist");
                return false;
            }

            if (CurrentState != null) CurrentState.Deactivate(unloadCurrent);
            CurrentState = nextState;
            CurrentState.Activate(loadScene);

            return true;
        }
    }
}
