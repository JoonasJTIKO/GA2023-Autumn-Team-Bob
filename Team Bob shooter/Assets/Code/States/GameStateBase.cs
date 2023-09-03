using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public abstract class GameStateBase
    {
        private List<StateType> targetStates = new List<StateType>();

        public abstract string SceneName { get; }
        public abstract StateType Type { get; }

        protected void AddTargetState(StateType type)
        {
            if (!targetStates.Contains(type))
            {
                targetStates.Add(type);
            }
        }

        public abstract void Activate(bool loadScene = true);
        public abstract void Deactivate(bool unloadScene = true);

        public bool IsValidTarget(StateType targetStateType)
        {
            foreach (StateType stateType in targetStates)
            {
                if (stateType == targetStateType)
                {
                    return true;
                }
            }

            return false;
        }

        protected GameStateBase()
        {
        }
    }
}
