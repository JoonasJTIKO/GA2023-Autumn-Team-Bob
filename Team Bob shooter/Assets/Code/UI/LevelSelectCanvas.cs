using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamBobFPS.UI
{
    public class LevelSelectCanvas : MenuCanvas
    {
        [SerializeField]
        private TMP_Text pageModeText;

        [SerializeField]
        private GameObject[] normalButtons;

        [SerializeField]
        private GameObject[] endlessButtons;

        [SerializeField]
        private HighscoreUI highscoreUI;

        private InputAction togglePageAction, backAction;

        private PlayerInputs playerInputs;

        private bool endlessPageActive = false;

        private bool pressed = false;

        private int selectedLevelIndex = 2;

        protected override void Awake()
        {
            base.Awake();

            playerInputs = new PlayerInputs();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (eventSystem.currentSelectedGameObject == endlessButtons[0] && selectedLevelIndex != 2)
            {
                selectedLevelIndex = 2;
                highscoreUI.Activate(selectedLevelIndex);

            }
            else if (eventSystem.currentSelectedGameObject == endlessButtons[1] && selectedLevelIndex != 3)
            {
                selectedLevelIndex = 3;
                highscoreUI.Activate(selectedLevelIndex);
            }
        }

        public override void Show()
        {
            base.Show();

            pressed = false;

            if (playerInputs != null)
            {
                playerInputs.Menu.Enable();
                togglePageAction = playerInputs.Menu.TogglePage;
                togglePageAction.Enable();
                togglePageAction.performed += TogglePage;
                backAction = playerInputs.Menu.Back;
                backAction.Enable();
                backAction.performed += GoBack;
            }

            StartCoroutine(Activate());
        }

        public override void Hide()
        {
            if (playerInputs != null)
            {
                playerInputs.Menu.Disable();
                togglePageAction.Disable();
                togglePageAction.performed -= TogglePage;
                backAction.Disable();
                backAction.performed -= GoBack;
            }

            foreach (var button in normalButtons)
            {
                button.SetActive(false);
            }

            foreach (var button in endlessButtons)
            {
                button.SetActive(false);
            }

            endlessPageActive = false;
            highscoreUI.gameObject.SetActive(false);

            base.Hide();
        }

        private void GoBack(InputAction.CallbackContext context)
        {
            Hide();
            GameInstance.Instance.GetLoadoutSelectCanvas().Show();
        }

        private void TogglePage(InputAction.CallbackContext context)
        {
            if (!endlessPageActive)
            {
                endlessPageActive = true;
                pageModeText.text = "Endless";
                highscoreUI.gameObject.SetActive(true);
                for (int i = 0; i < normalButtons.Length; i++)
                {
                    normalButtons[i].SetActive(false);
                    endlessButtons[i].SetActive(true);
                }
                eventSystem.SetSelectedGameObject(endlessButtons[0]);
            }
            else
            {
                endlessPageActive = false;
                pageModeText.text = "Standard";
                highscoreUI.gameObject.SetActive(false);
                for (int i = 0; i < normalButtons.Length; i++)
                {
                    normalButtons[i].SetActive(true);
                    endlessButtons[i].SetActive(false);
                }
                eventSystem.SetSelectedGameObject(normalButtons[0]);
            }
        }

        private IEnumerator Activate()
        {
            yield return new WaitForFixedUpdate();
            foreach (var button in normalButtons)
            {
                button.SetActive(true);
            }
        }

        private IEnumerator LoadLevelAfterDelay(float delay, StateType stateType)
        {
            float timer = 0f;
            while (timer < delay)
            {
                timer += Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale;
                yield return null;
            }

            GameInstance.Instance.GetGameStateManager().Go(stateType);
        }

        public void LoadLevel1Normal()
        {
            if (!pressed)
            {
                pressed = true;

                GameInstance.Instance.GetFadeCanvas().FadeTo(0.5f);
                StartCoroutine(LoadLevelAfterDelay(0.5f, StateType.Arena1));
            }
        }

        public void LoadLevel2Normal()
        {
            if (!pressed)
            {
                pressed = true;

                GameInstance.Instance.GetFadeCanvas().FadeTo(0.5f);
                StartCoroutine(LoadLevelAfterDelay(0.5f, StateType.Arena1));
            }
        }

        public void LoadLevel1Endless()
        {
            if (!pressed)
            {
                pressed = true;

                GameInstance.Instance.GetFadeCanvas().FadeTo(0.5f);
                StartCoroutine(LoadLevelAfterDelay(0.5f, StateType.Arena1Endless));
            }
        }

        public void LoadLevel2Endless()
        {
            if (!pressed)
            {
                pressed = true;

                GameInstance.Instance.GetFadeCanvas().FadeTo(0.5f);
                StartCoroutine(LoadLevelAfterDelay(0.5f, StateType.Arena1Endless));
            }
        }
    }
}
