using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace TeamBobFPS
{
    [System.Serializable]
    public class SaveSettings : MonoBehaviour
    {
        public static void SaveGameSettings()
        {
            string path = Application.persistentDataPath + "/settings.wtf";

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(stream, SettingsData.settings);
            }
        }

        public static void LoadSettings()
        {
            string path = Application.persistentDataPath + "/settings.wtf";
            BinaryFormatter formatter = new BinaryFormatter();

            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    SettingsClass savedSettings = formatter.Deserialize(stream) as SettingsClass;
                    SettingsData.settings = savedSettings;
                }
            }

            else
            {
                print("settings file not found, creating new");
                SettingsClass defaultSettings = new SettingsClass(0.5f, 0.5f, 0.5f, 2, true, true);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(stream, defaultSettings);
                }
            }
        }
    }
}