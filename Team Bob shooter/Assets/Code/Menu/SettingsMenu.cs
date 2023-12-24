using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class SettingsMenu : MonoBehaviour
    {
        public TMPro.TMP_Dropdown resolutionDropdown;
        SettingsLoader loader;
        private bool inMenu = false;

        [SerializeField] private Color selected;
        [SerializeField] private Color unselected;
        [SerializeField] private Canvas audioCanvas;
        [SerializeField] private Canvas graphicsCanvas;
        private GameObject myEventSystem;
        [SerializeField] private GameObject selectedSlider;
        [SerializeField] private GameObject resolution;
        [SerializeField] private Sprite[] inputIcons;

        public void Awake()
        {
            myEventSystem = GameObject.Find("EventSystem"); 
            GameObject lol = GameObject.FindGameObjectWithTag("SettingsLoader");
            loader = lol.GetComponent<SettingsLoader>();
            SaveSettings.LoadSettings();
            resolutionDropdown.value = SettingsData.settings.resolutionIndex;
        }

        public void Close()
        {
            StartCoroutine("ActualClose");
        }

        private IEnumerator ActualClose()
        {
            yield return new WaitForSecondsRealtime(0.15f);
        }

        public void SetResolution(int index)
        {
            loader.SetResolution(index);
        }

        public void QuitGame()
        {
            print("reloading menu for save reset");
        }

        public void ResetSelection()
        {
            if (myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().currentSelectedGameObject != null) { return; }
            else if (audioCanvas.enabled) { myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(selectedSlider); }
            else if (graphicsCanvas.enabled) { myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(resolution); }
        }
    }
}
