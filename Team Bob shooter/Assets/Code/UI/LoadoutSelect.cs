using System.Collections;
using System.Collections.Generic;
using TeamBobFPS.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TeamBobFPS
{
    public class LoadoutSelect : MonoBehaviour
    {
        [SerializeField]
        private EquippableWeapon weapon;

        [SerializeField]
        private string weaponDetails;

        private Button button;

        private LoadoutSelectCanvas loadoutSelectCanvas;

        private void Awake()
        {
            button = GetComponent<Button>();
            loadoutSelectCanvas = GetComponentInParent<LoadoutSelectCanvas>();
        }

        private void Start()
        {
            StartCheck();
        }

        private void OnEnable()
        {
            WeaponLoadout.WeaponUnequipped += CheckOnUnequip;
        }

        private void OnDisable()
        {
            WeaponLoadout.WeaponUnequipped -= CheckOnUnequip;
        }

        /// <summary>
        /// Deactivates this if the weapon is equipped by the player or not unlocked
        /// </summary>
        private void StartCheck()
        {
            if (GameInstance.Instance == null) return;

            if (GameInstance.Instance.GetWeaponLoadout().CheckEquipStatus(weapon) == true
                || !GameInstance.Instance.GetGameProgressionManager().CheckWeaponUnlockState(weapon))
            {
                button.interactable = false;
            }
            loadoutSelectCanvas.SetSelectedObject();
        }

        /// <summary>
        /// Called when the player equips another weapon unequipping their current weapon. Enables this if this weapon was unequipped
        /// </summary>
        /// <param name="weapon"></param>
        private void CheckOnUnequip(EquippableWeapon weapon)
        {
            if (weapon.WeaponType == this.weapon.WeaponType)
            {
                button.interactable = true;
            }
        }

        public void OnSelect()
        {
            loadoutSelectCanvas.EnableSlotSelect(weapon);
            loadoutSelectCanvas.WriteDetails(weaponDetails);
            button.interactable = false;
        }

        public IEnumerator Enable()
        {
            yield return null;

            button.interactable = true;
            StartCheck();
        }
    }
}
