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
        private TextWriteOverTime textWrite;

        [SerializeField]
        private LoadoutSlotSelect slotSelect1;

        [SerializeField]
        private LoadoutSlotSelect slotSelect2;

        [SerializeField]
        private Button[] LoadoutButtons;

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

        public void SetSelectedObject(GameObject gameObject = null)
        {

            if (gameObject == null)
            {
                eventSystem.SetSelectedGameObject(initialSelectedObject);
                return;
            }

            eventSystem.SetSelectedGameObject(gameObject);
        }

        public void WriteDetails(string text)
        {
            textWrite.StartWrite(text);
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
        }

        private IEnumerator WaitForInputRelease()
        {
            while (selectAction.phase == InputActionPhase.Performed)
            {
                yield return null;
            }

            eventSystem.enabled = true;
        }
    }
}
