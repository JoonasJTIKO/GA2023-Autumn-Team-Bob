using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.Save;
using UnityEngine;

namespace TeamBobFPS
{
    public class GameProgressionManager : MonoBehaviour, ISaveable
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

        public Dictionary<string, int> VillageEndlessModeHighScores
        {
            get;
            private set;
        }

        public SaveObjectType SaveType
        { get { return SaveObjectType.GameProgression; } }

        private void Awake()
        {
            CurrentGameProgress = 0;

            VillageEndlessModeHighScores = new Dictionary<string, int>();
            VillageEndlessModeHighScores.Add("Pistol", 0);
            VillageEndlessModeHighScores.Add("Shotgun", 0);
            VillageEndlessModeHighScores.Add("Minigun", 0);
            VillageEndlessModeHighScores.Add("Railgun", 0);
        }

        public void UpdateGameProgress(int levelIndex)
        {
            CurrentGameProgress = (GameProgress)levelIndex;

            weaponUnlockStates[levelIndex + 1].unlocked = true;
            GameInstance.Instance.GetSaveController().QuickSave();
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

        public void UpdateEndlessHighscore(int levelIndex)
        {
            string weapon1 = GameInstance.Instance.GetWeaponLoadout().EquippedWeapons[0].WeaponType.ToString();
            string weapon2 = GameInstance.Instance.GetWeaponLoadout().EquippedWeapons[1].WeaponType.ToString();

            switch (levelIndex)
            {
                case 2:
                    VillageEndlessModeHighScores[weapon1]++;
                    VillageEndlessModeHighScores[weapon2]++;
                    GameInstance.Instance.GetSaveController().QuickSave();
                    break;
            }
        }

        public void Save(ISaveWriter writer)
        {
            writer.WriteInt((int)SaveType);
            writer.WriteInt((int)CurrentGameProgress);

            foreach(var weaponUnlockState in weaponUnlockStates)
            {
                writer.WriteBool(weaponUnlockState.unlocked);
            }

            foreach(var key in VillageEndlessModeHighScores)
            {
                writer.WriteInt(key.Value);
            }

        }

        public void Load(ISaveReader reader)
        {
            int CurrentGameProgress = reader.ReadInt();

            foreach (var weaponUnlockState in weaponUnlockStates)
            {
                weaponUnlockState.unlocked = reader.ReadBool();
            }

            foreach (var key in VillageEndlessModeHighScores)
            {
                VillageEndlessModeHighScores[key.Key] = reader.ReadInt();
            }
        }
    }
}
