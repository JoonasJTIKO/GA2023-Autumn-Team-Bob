using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    [System.Serializable]
    public class SettingsClass
    {
        public float musicVolume;
        public float sfxVolume;
        public float ambVolume;
        public int resolutionIndex;
        public bool fullscreen;
        public bool renderClouds;
        public bool movementWobble;
        public bool lowresToggle;
        public bool alarmMute;

        public SettingsClass(float mv, float sv, float av, int res, bool fs, bool lr)
        {
            musicVolume = mv;
            sfxVolume = sv;
            ambVolume = av;
            resolutionIndex = res;
            fullscreen = fs;
            lowresToggle = lr;
        }
    }
}
