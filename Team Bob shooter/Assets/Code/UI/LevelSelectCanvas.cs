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

        private InputAction togglePage;

        private PlayerInputs playerInputs;

        private bool endlessPageActive = false;

        private bool pressed = false;

        protected override void Awake()
        {
            base.Awake();

            playerInputs = new PlayerInputs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            pressed = false;

            if (playerInputs != null )
            {
                playerInputs.Menu.Enable();
                togglePage = playerInputs.Menu.TogglePage;
                togglePage.Enable();
                togglePage.performed += TogglePage;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();


            if (playerInputs != null)
            {
                playerInputs.Menu.Disable();
                togglePage.Disable();
                togglePage.performed -= TogglePage;
            }
        }

        public override void Show()
        {
            base.Show();

            StartCoroutine(Activate());
        }

        private void TogglePage(InputAction.CallbackContext context)
        {
            if (!endlessPageActive)
            {
                endlessPageActive = true;
                pageModeText.text = "Endless";
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
            //int counter = 0;
            //while (counter < 5)
            //{
            //    yield return null;
            //    counter++;
            //}

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
