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
        public Toggle fstoggle;
        SettingsLoader loader;
        private bool inMenu = false;
        [SerializeField] private Transform boxTf;

        [SerializeField] private Canvas options;
        [SerializeField] private Canvas confirmation;
        [SerializeField] private Canvas forcequit;
        [SerializeField] private Color selected;
        [SerializeField] private Color unselected;
        [SerializeField] private Canvas audioCanvas;
        [SerializeField] private Canvas graphicsCanvas;
        private GameObject myEventSystem;
        [SerializeField] private GameObject selectedSlider;
        [SerializeField] private GameObject resolution;
        [SerializeField] private Toggle lowresToggleButton;
        [SerializeField] private Sprite[] inputIcons;

        private bool isController;

        public void Awake()
        {
            myEventSystem = GameObject.Find("EventSystem");
            //menuControls = new InputMaster();
            //menuControls.PlayerControl.Seedselectionleft.performed += _ => OpenAudioButton();
            //menuControls.PlayerControl.Seedselectionright.performed += _ => OpenGraphicsButton();
            //menuControls.PlayerControl.Navigate.performed += _ => ResetSelection();
            //menuControls.PlayerControl.Navigate1.performed += _ => ResetSelection();
            //menuControls.PlayerControl.Cancel.performed += _ => Close();
            GameObject lol = GameObject.FindGameObjectWithTag("Level loader");
            loader = lol.GetComponent<SettingsLoader>();
            SaveSettings.LoadSettings();
            fstoggle.isOn = SettingsData.settings.fullscreen;
            resolutionDropdown.value = SettingsData.settings.resolutionIndex;
            lowresToggleButton.isOn = SettingsData.settings.lowresToggle;

            //if (GameStateManager.Instance.PreviousState.Type == StateType.MainMenu) { inMenu = true; }
        }

        void Update()
        {

        }

        public void Close()
        {
            StartCoroutine("ActualClose");
        }

        private IEnumerator ActualClose()
        {
            yield return new WaitForSecondsRealtime(0.15f);
            //GameStateManager.Instance.GoBack();
            //if (inMenu) { GameObject.FindGameObjectWithTag("Main menu master").GetComponent<UIMainMenu>().CloseOptions(); }
        }

        public void Fullscreen(bool isFS)
        {
            loader.Fullscreen(isFS);
        }

        public void SetResolution(int index)
        {
            loader.SetResolution(index);
        }

        public void DownresToggle(bool toggle)
        {
            loader.LowRes(toggle);
        }

        public void QuitGame()
        {
            //GameStateManager.Instance.GoBack();
            print("reloading menu for save reset");
        }

        void OpenAudioButton()
        {
            OpenAudio();
        }
        public void OpenAudio()
        {
            if (audioCanvas.enabled == false)
            {
                //myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                //AudioIcon.color = Color.white;
                //AudioSelection.color = selected;
                //graphicsIcon.color = unselected;
                //graphicsSelection.color = unselected;
                //graphicsCanvas.enabled = false;
                //audioCanvas.enabled = true;
            }
        }

        void OpenGraphicsButton()
        {
            //OpenGraphics();
        }

        //public void OpenGraphics()
        //{
        //    if (graphicsCanvas.enabled == false)
        //    {
        //        sfx.OnClick();
        //        graphicsSelectionAnim.Animate();
        //        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        //        graphicsSelection.color = selected;
        //        graphicsIcon.color = Color.white;
        //        AudioIcon.color = unselected;
        //        AudioSelection.color = unselected;
        //        audioCanvas.enabled = false;
        //        graphicsCanvas.enabled = true;
        //    }
        //}

        public void ResetSelection()
        {
            if (myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().currentSelectedGameObject != null) { return; }
            else if (audioCanvas.enabled) { myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(selectedSlider); }
            else if (graphicsCanvas.enabled) { myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(resolution); }
        }
        private void OnEnable()
        {
            //menuControls.Enable();
        }

        private void OnDisable()
        {
            //menuControls.Disable();
        }
    }
}
