using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace TeamBobFPS.UI
{
    public class MainMenu : MenuCanvas
    {
        [SerializeField]
        private GameObject settingsInitialSelected;

        [SerializeField]
        private Animator holderAnimator;

        public Canvas mainCanvas;
        public Canvas settingsCanvas;

        private PlayerInputs playerInputs;
        private InputAction selectAction;

        protected override void Awake()
        {
            base.Awake();

            playerInputs = new PlayerInputs();
            selectAction = playerInputs.Menu.Select;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            selectAction.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            selectAction?.Disable();
        }

        public override void Show()
        {
            base.Show();
            mainCanvas.gameObject.SetActive(true);
            holderAnimator.SetTrigger("Appear");
        }

        public override void Hide()
        {
            StartCoroutine(Disable());
            holderAnimator.SetTrigger("Dissapear");
        }

        public void OpenOptions()
        {
            settingsCanvas.gameObject.SetActive(true);
            mainCanvas.gameObject.SetActive(false);
            eventSystem.SetSelectedGameObject(settingsInitialSelected);
        }

        public void PlayGame()
        {
            Hide();
            GameInstance.Instance.GetLoadoutSelectCanvas().Show();
        }

        public void Quit()
        {
            Application.Quit();
            Debug.Log("Quit");
        }

        public void Close()
        {
            settingsCanvas.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
            eventSystem.SetSelectedGameObject(initialSelectedObject);
            eventSystem.enabled = false;
            StartCoroutine(WaitForInputRelease());
        }

        private IEnumerator WaitForInputRelease()
        {
            while (selectAction.phase == InputActionPhase.Performed)
            {
                yield return null;
            }

            eventSystem.enabled = true;
        }

        private IEnumerator Disable()
        {
            yield return new WaitForSeconds(0.84f);

            mainCanvas.gameObject.SetActive(false);
        }
    }
}
