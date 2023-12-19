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
        public int resolutionIndex;
        public bool fullscreen;

        public SettingsClass(float mv, float sv, int res, bool fs)
        {
            musicVolume = mv;
            sfxVolume = sv;
            resolutionIndex = res;
            fullscreen = fs;
        }
    }
}
