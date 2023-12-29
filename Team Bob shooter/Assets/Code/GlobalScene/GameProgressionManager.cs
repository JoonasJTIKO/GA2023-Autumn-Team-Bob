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

        private Dictionary<string, int> currentVillageEndlessModeHighScores;

        public Dictionary<string, int> TempleEndlessModeHighScores
        {
            get;
            private set;
        }

        private Dictionary<string, int> currentTempleEndlessModeHighScores;

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

            currentVillageEndlessModeHighScores = new Dictionary<string, int>();
            currentVillageEndlessModeHighScores.Add("Pistol", 0);
            currentVillageEndlessModeHighScores.Add("Shotgun", 0);
            currentVillageEndlessModeHighScores.Add("Minigun", 0);
            currentVillageEndlessModeHighScores.Add("Railgun", 0);

            TempleEndlessModeHighScores = new Dictionary<string, int>();
            TempleEndlessModeHighScores.Add("Pistol", 0);
            TempleEndlessModeHighScores.Add("Shotgun", 0);
            TempleEndlessModeHighScores.Add("Minigun", 0);
            TempleEndlessModeHighScores.Add("Railgun", 0);

            currentTempleEndlessModeHighScores = new Dictionary<string, int>();
            currentTempleEndlessModeHighScores.Add("Pistol", 0);
            currentTempleEndlessModeHighScores.Add("Shotgun", 0);
            currentTempleEndlessModeHighScores.Add("Minigun", 0);
            currentTempleEndlessModeHighScores.Add("Railgun", 0);
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

        public void ResetEndless()
        {
            currentVillageEndlessModeHighScores = new Dictionary<string, int>();
            currentVillageEndlessModeHighScores.Add("Pistol", 0);
            currentVillageEndlessModeHighScores.Add("Shotgun", 0);
            currentVillageEndlessModeHighScores.Add("Minigun", 0);
            currentVillageEndlessModeHighScores.Add("Railgun", 0);

            currentTempleEndlessModeHighScores = new Dictionary<string, int>();
            currentTempleEndlessModeHighScores.Add("Pistol", 0);
            currentTempleEndlessModeHighScores.Add("Shotgun", 0);
            currentTempleEndlessModeHighScores.Add("Minigun", 0);
            currentTempleEndlessModeHighScores.Add("Railgun", 0);
        }

        public void UpdateEndlessHighscore(int levelIndex)
        {
            string weapon1 = GameInstance.Instance.GetWeaponLoadout().EquippedWeapons[0].WeaponType.ToString();
            string weapon2 = GameInstance.Instance.GetWeaponLoadout().EquippedWeapons[1].WeaponType.ToString();

            switch (levelIndex)
            {
                case 2:
                    currentVillageEndlessModeHighScores[weapon1]++;
                    currentVillageEndlessModeHighScores[weapon2]++;
                    if (currentVillageEndlessModeHighScores[weapon1] > VillageEndlessModeHighScores[weapon1])
                    {
                        VillageEndlessModeHighScores[weapon1] = currentVillageEndlessModeHighScores[weapon1];
                    }
                    if (currentVillageEndlessModeHighScores[weapon2] > VillageEndlessModeHighScores[weapon2])
                    {
                        VillageEndlessModeHighScores[weapon2] = currentVillageEndlessModeHighScores[weapon2];
                    }
                    GameInstance.Instance.GetSaveController().QuickSave();
                    break;
                case 3:
                    currentTempleEndlessModeHighScores[weapon1]++;
                    currentTempleEndlessModeHighScores[weapon2]++;
                    if (currentTempleEndlessModeHighScores[weapon1] > TempleEndlessModeHighScores[weapon1])
                    {
                        TempleEndlessModeHighScores[weapon1] = currentTempleEndlessModeHighScores[weapon1];
                    }
                    if (currentTempleEndlessModeHighScores[weapon2] > TempleEndlessModeHighScores[weapon2])
                    {
                        TempleEndlessModeHighScores[weapon2] = currentTempleEndlessModeHighScores[weapon2];
                    }
                    GameInstance.Instance.GetSaveController().QuickSave();
                    break;
            }
        }

        public void Save(ISaveWriter writer)
        {
            writer.WriteInt((int)SaveType);
            writer.WriteInt((int)CurrentGameProgress);

            foreach (var weaponUnlockState in weaponUnlockStates)
            {
                writer.WriteBool(weaponUnlockState.unlocked);
            }

            foreach (var key in VillageEndlessModeHighScores)
            {
                writer.WriteInt(key.Value);
            }

            foreach (var key in TempleEndlessModeHighScores)
            {
                writer.WriteInt(key.Value);
            }
        }

        public void Load(ISaveReader reader)
        {
            CurrentGameProgress = (GameProgress)reader.ReadInt();

            foreach (var weaponUnlockState in weaponUnlockStates)
            {
                weaponUnlockState.unlocked = reader.ReadBool();
            }

            VillageEndlessModeHighScores["Pistol"] = reader.ReadInt();
            VillageEndlessModeHighScores["Shotgun"] = reader.ReadInt();
            VillageEndlessModeHighScores["Minigun"] = reader.ReadInt();
            VillageEndlessModeHighScores["Railgun"] = reader.ReadInt();

            TempleEndlessModeHighScores["Pistol"] = reader.ReadInt();
            TempleEndlessModeHighScores["Shotgun"] = reader.ReadInt();
            TempleEndlessModeHighScores["Minigun"] = reader.ReadInt();
            TempleEndlessModeHighScores["Railgun"] = reader.ReadInt();
        }
    }
}
