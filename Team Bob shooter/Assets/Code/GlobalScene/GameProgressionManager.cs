using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class GameProgressionManager : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponUnlockState
        {
            public EquippableWeapon weapon;
            public bool unlocked = false;
        }

        [SerializeField]
        private WeaponUnlockState[] weaponUnlockStates;

        public enum GameProgress
        {
            NoneCleared = 0,
            FirstCleared = 1,
            SecondCleared = 2,
            AllCleared = 3,
        }

        public GameProgress CurrentGameProgress
        {
            get;
            private set;
        }

        private void Awake()
        {
            CurrentGameProgress = 0;
        }

        public void UpdateGameProgress(int levelIndex)
        {
            CurrentGameProgress = (GameProgress)levelIndex;

            weaponUnlockStates[levelIndex + 1].unlocked = true;
        }

        public bool CheckWeaponUnlockState(EquippableWeapon weapon)
        {
            foreach (var state in weaponUnlockStates)
            {
                if (state.weapon.WeaponType == weapon.WeaponType)
                {
                    return state.unlocked;
                }
            }
            Debug.LogError("Weapon not added to progression manager!");
            return false;
        }
    }
}
