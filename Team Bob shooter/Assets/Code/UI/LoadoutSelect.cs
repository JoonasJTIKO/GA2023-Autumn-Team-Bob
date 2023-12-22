using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamBobFPS.UI
{
    public class LoadoutSelect : MonoBehaviour, ISelectHandler
    {
        [SerializeField]
        private EquippableWeapon weapon;

        [SerializeField]
        private string weaponName;

        [SerializeField]
        private string weaponDesc;

        [SerializeField]
        private string weaponMovement;

        [SerializeField]
        private int modelIndex;

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
            StartCheck();
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

            if (/*GameInstance.Instance.GetWeaponLoadout().CheckEquipStatus(weapon) == true*/
                /*||*/ !GameInstance.Instance.GetGameProgressionManager().CheckWeaponUnlockState(weapon))
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

        public void OnSelect(BaseEventData eventData)
        {
            loadoutSelectCanvas.SetModel(modelIndex);
            loadoutSelectCanvas.WriteName(weaponName);
            loadoutSelectCanvas.WriteDesc(weaponDesc);
            loadoutSelectCanvas.WriteMovement(weaponMovement);
        }

        public void OnPress()
        {
            loadoutSelectCanvas.EnableSlotSelect(weapon);
            button.interactable = false;

            GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(EGameSFX._SFX_UI_PRESS, transform.position, make2D: true);
        }

        public IEnumerator Enable()
        {
            yield return null;

            button.interactable = true;
            StartCheck();
        }
    }
}
