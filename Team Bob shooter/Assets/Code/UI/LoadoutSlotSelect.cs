using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class LoadoutSlotSelect : MonoBehaviour
    {
        [SerializeField]
        private int slotIndex;

        private EquippableWeapon weapon;

        private Button button;

        private TMP_Text text;

        private LoadoutSelectCanvas loadoutSelectCanvas;

        private void Awake()
        {
            button = GetComponent<Button>();
            loadoutSelectCanvas = GetComponentInParent<LoadoutSelectCanvas>();
            text = GetComponentInChildren<TMP_Text>();
        }

        public void SetWeapon(EquippableWeapon weapon)
        {
            this.weapon = weapon;
            StartCoroutine(Enable());
        }

        private IEnumerator Enable()
        {
            yield return null;

            button.interactable = true;
            loadoutSelectCanvas.SetSelectedObject(gameObject);
        }

        public void Disable()
        {
            button.interactable = false;
        }

        public void EquipToSlot()
        {
            if (weapon == null) return;
            GameInstance.Instance.GetWeaponLoadout().EquipWeapon(weapon, slotIndex);
            UpdateText(weapon.WeaponType);
            weapon = null;
            loadoutSelectCanvas.DisableSlotSelect();
        }

        private void UpdateText(EquippableWeapon.Weapon weaponType)
        {
            switch (weaponType)
            {
                case EquippableWeapon.Weapon.Pistol:
                    text.text = "Pistol";
                    break;
                case EquippableWeapon.Weapon.Shotgun:
                    text.text = "Shotgun";
                    break;
                case EquippableWeapon.Weapon.Minigun:
                    text.text = "Minigun";
                    break;
                case EquippableWeapon.Weapon.Railgun:
                    text.text = "Railgun";
                    break;
                case EquippableWeapon.Weapon.RocketLauncher:
                    text.text = "Rocket Launcher";
                    break;
            }
        }
    }
}
