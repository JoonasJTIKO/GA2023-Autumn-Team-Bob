using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class SettingsLoader : MonoBehaviour
    {
        void Awake()
        {
            SaveSettings.LoadSettings();
            Fullscreen(SettingsData.settings.fullscreen);
            SetResolution(SettingsData.settings.resolutionIndex);
            LowRes(SettingsData.settings.lowresToggle);
        }

        public void Fullscreen(bool isFS)
        {
            Screen.fullScreen = isFS;
            SettingsData.settings.fullscreen = isFS;
            SaveSettings.SaveGameSettings();
        }

        public void Wobble(bool wobble)
        {
            SettingsData.settings.movementWobble = wobble;
            SaveSettings.SaveGameSettings();
        }

        public void LowRes(bool resToggle)
        {
            //if (camFollowScript != null) { camFollowScript.ToggleDownRes(resToggle); }
            //SettingsData.settings.lowresToggle = resToggle;
            //SaveSettings.SaveGameSettings();
        }

        public void CloudToggle(bool cloudToggleValue)
        {
            SettingsData.settings.renderClouds = cloudToggleValue;
            SaveSettings.SaveGameSettings();
        }

        public void SetResolution(int index)
        {
            switch (index)
            {
                case 0:
                    Screen.SetResolution(960, 540, Screen.fullScreen);
                    break;
                case 1:
                    Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
                case 2:
                    Screen.SetResolution(1600, 900, Screen.fullScreen);
                    break;
                case 3:
                    Screen.SetResolution(1920, 1080, Screen.fullScreen);
                    break;
                case 4:
                    Screen.SetResolution(2560, 1440, Screen.fullScreen);
                    break;
                case 5:
                    Screen.SetResolution(3840, 2160, Screen.fullScreen);
                    break;
                default:
                    break;
            }
            SettingsData.settings.resolutionIndex = index;
            SaveSettings.SaveGameSettings();
        }
    }
}
