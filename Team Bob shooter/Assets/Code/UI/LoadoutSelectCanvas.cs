using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TeamBobFPS.UI
{
    public class LoadoutSelectCanvas : MenuCanvas
    {
        [SerializeField]
        private TextWriteOverTime nameWrite;

        [SerializeField]
        private TextWriteOverTime descWrite;

        [SerializeField]
        private TextWriteOverTime movementWrite;

        [SerializeField]
        private LoadoutSlotSelect slotSelect1;

        [SerializeField]
        private LoadoutSlotSelect slotSelect2;

        [SerializeField]
        private Button[] LoadoutButtons;

        [SerializeField]
        private GameObject weaponModel;

        [SerializeField]
        private GameObject weaponModelCamera;

        [SerializeField]
        private Animator holderAnimator;

        [SerializeField]
        private Canvas background;

        private LoadoutSelectWeaponModel LoadoutSelectWeaponModel;

        private PlayerInputs playerInputs;

        private InputAction selectAction, backAction;

        protected override void Awake()
        {
            base.Awake();

            playerInputs = new PlayerInputs();
            selectAction = playerInputs.Menu.Select;
            backAction = playerInputs.Menu.Back;

            LoadoutSelectWeaponModel = weaponModel.GetComponent<LoadoutSelectWeaponModel>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            selectAction.Enable();
            backAction.Enable();
            backAction.performed += GoBack;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            selectAction?.Disable();
            backAction?.Disable();
            backAction.performed -= GoBack;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(Activate());
        }

        private IEnumerator Activate()
        {
            yield return new WaitForFixedUpdate();
            base.Show();

            holderAnimator.SetTrigger("Appear");
            weaponModel.SetActive(true);
            weaponModelCamera.SetActive(true);
            background.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            //base.Hide();
            StartCoroutine(Disable());
            holderAnimator.SetTrigger("Dissapear");
            weaponModel?.SetActive(false);
            weaponModelCamera?.SetActive(false);
            background.gameObject.SetActive(false);
        }

        private void GoBack(InputAction.CallbackContext context)
        {
            Hide();
            GameInstance.Instance.GetMainMenu().Show();

            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_UI_PRESS, transform.position, make2D: true);
        }

        public void SetSelectedObject(GameObject gameObject = null)
        {

            if (gameObject == null)
            {
                eventSystem.SetSelectedGameObject(initialSelectedObject);
                return;
            }

            eventSystem.SetSelectedGameObject(gameObject);
        }

        public void SetModel(int modelIndex)
        {
            LoadoutSelectWeaponModel.EnableModel(modelIndex);
        }

        public void WriteName(string text)
        {
            nameWrite.StartWrite(text);
        }

        public void WriteDesc(string text)
        {
            descWrite.StartWrite(text);
        }

        public void WriteMovement(string text)
        {
            movementWrite.StartWrite(text);
        }

        public void EnableSlotSelect(EquippableWeapon weapon)
        {
            eventSystem.enabled = false;
            StartCoroutine(WaitForInputRelease());
            slotSelect1.SetWeapon(weapon);
            slotSelect2.SetWeapon(weapon);
        }

        public void DisableSlotSelect()
        {
            eventSystem.enabled = false;
            StartCoroutine(WaitForInputRelease());
            slotSelect1.Disable();
            slotSelect2.Disable();
            foreach (var button in LoadoutButtons)
            {
                StartCoroutine(button.GetComponent<LoadoutSelect>().Enable());
            }
        }

        public void EnterLevelSelect()
        {
            Hide();
            GameInstance.Instance.GetLevelSelectCanvas().Show();

            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_UI_PRESS, transform.position, make2D: true);
        }

        private IEnumerator WaitForInputRelease()
        {
            while (selectAction.phase == InputActionPhase.Performed)
            {
                yield return null;
            }

            eventSystem.enabled = true;
            buttonPressed = false;
        }

        private IEnumerator Disable()
        {
            yield return new WaitForSeconds(0.84f);

            gameObject.SetActive(false);
        }
    }
}
