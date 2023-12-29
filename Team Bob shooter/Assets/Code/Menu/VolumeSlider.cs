using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class VolumeSlider : MonoBehaviour
    {
        public AudioMixer mixer;
        public Slider slider;

        public void Initialize()
        {
            if (gameObject.name == "Music Slider")
            {
                slider.value = SettingsData.settings.musicVolume;
            }
            else if (gameObject.name == "SFX Slider")
            {
                slider.value = SettingsData.settings.sfxVolume;
            }
        }

        public void SetVolume(float volume)
        {
            if (gameObject.name == "Music Slider")
            {
                mixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
                SettingsData.settings.musicVolume = volume;
            }
            else if (gameObject.name == "SFX Slider")
            {
                mixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
                SettingsData.settings.sfxVolume = volume;
            }

            SaveSettings.SaveGameSettings();
        }
    }
}
